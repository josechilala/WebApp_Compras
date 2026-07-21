using WebAPP_Compras.Models.DTOs.Orders;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Models.Enums;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public sealed class OrderService : IOrderService
{
    private const decimal DefaultDeliveryFee = 10m;

    private readonly IOrderRepository _orderRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IDeliveryScheduleRepository
        _deliveryScheduleRepository;
    private readonly ICurrentUserService _currentUserService;

    public OrderService(
        IOrderRepository orderRepository,
        IStoreRepository storeRepository,
        IProductRepository productRepository,
        IAddressRepository addressRepository,
        IDeliveryScheduleRepository deliveryScheduleRepository,
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _storeRepository = storeRepository;
        _productRepository = productRepository;
        _addressRepository = addressRepository;
        _deliveryScheduleRepository =
            deliveryScheduleRepository;
        _currentUserService = currentUserService;
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        int userId = _currentUserService.UserId;

        Store? store = await _storeRepository.GetByIdAsync(
            request.StoreId,
            includeInactive: false,
            cancellationToken);

        if (store is null)
        {
            throw new KeyNotFoundException(
                "Mercado não encontrado ou inativo.");
        }

        Address? address =
            await _addressRepository
                .GetActiveByIdForUserAsync(
                    request.AddressId,
                    userId,
                    cancellationToken);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Endereço não encontrado para o usuário autenticado.");
        }

        DeliverySchedule? schedule =
            await _deliveryScheduleRepository
                .GetAvailableTrackedByIdAsync(
                    request.DeliveryScheduleId,
                    cancellationToken);

        ValidateDeliverySchedule(schedule);

        List<CreateOrderItemRequest> groupedItems =
            request.Items
                .GroupBy(item => item.ProductId)
                .Select(group => new CreateOrderItemRequest
                {
                    ProductId = group.Key,
                    Quantity = group.Sum(
                        item => item.Quantity)
                })
                .ToList();

        if (groupedItems.Any(item => item.Quantity > 100))
        {
            throw new ArgumentException(
                "A quantidade máxima permitida por produto é 100.");
        }

        int[] productIds = groupedItems
            .Select(item => item.ProductId)
            .ToArray();

        IReadOnlyCollection<Product> products =
            await _productRepository
                .GetAvailableByIdsAsync(
                    productIds,
                    request.StoreId,
                    cancellationToken);

        if (products.Count != productIds.Length)
        {
            throw new InvalidOperationException(
                "Um ou mais produtos não existem, estão indisponíveis " +
                "ou não pertencem ao mercado selecionado.");
        }

        DateTime utcNow = DateTime.UtcNow;

        List<OrderItem> orderItems = [];

        foreach (CreateOrderItemRequest requestedItem
                 in groupedItems)
        {
            Product product = products.Single(
                currentProduct =>
                    currentProduct.Id ==
                    requestedItem.ProductId);

            decimal subtotal =
                product.Price * requestedItem.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = requestedItem.Quantity,
                UnitPrice = product.Price,
                Subtotal = subtotal,
                CreatedAt = utcNow,
                IsActive = true
            };

            orderItems.Add(orderItem);
        }

        decimal productsAmount = orderItems.Sum(
            item => item.Subtotal);

        decimal deliveryFee = DefaultDeliveryFee;

        var order = new Order
        {
            UserId = userId,
            StoreId = store.Id,
            AddressId = address.Id,
            DeliveryScheduleId = schedule!.Id,
            Status = OrderStatus.Pending,
            PaymentMethod = request.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            TotalAmount = productsAmount + deliveryFee,
            DeliveryFee = deliveryFee,
            Notes = NormalizeOptionalText(request.Notes),
            CreatedAt = utcNow,
            IsActive = true,
            Items = orderItems
        };

        schedule.ReservedOrders++;
        schedule.UpdatedAt = utcNow;

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        Order? createdOrder =
            await _orderRepository.GetByIdAsync(
                order.Id,
                cancellationToken);

        if (createdOrder is null)
        {
            throw new InvalidOperationException(
                "O pedido foi criado, mas não pôde ser carregado.");
        }

        return MapToResponse(createdOrder);
    }

    public async Task<IReadOnlyCollection<OrderResponse>>
        GetMyOrdersAsync(
            CancellationToken cancellationToken = default)
    {
        int userId = _currentUserService.UserId;

        IReadOnlyCollection<Order> orders =
            await _orderRepository.GetAllAsync(
                userId,
                storeId: null,
                status: null,
                cancellationToken);

        return orders
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<IReadOnlyCollection<OrderResponse>>
        GetAllAsync(
            int? userId,
            int? storeId,
            OrderStatus? status,
            CancellationToken cancellationToken = default)
    {
        EnsureAdmin();

        if (userId.HasValue && userId.Value <= 0)
        {
            throw new ArgumentException(
                "O identificador do usuário é inválido.");
        }

        if (storeId.HasValue && storeId.Value <= 0)
        {
            throw new ArgumentException(
                "O identificador do mercado é inválido.");
        }

        if (status.HasValue &&
            !Enum.IsDefined(status.Value))
        {
            throw new ArgumentException(
                "O status informado é inválido.");
        }

        IReadOnlyCollection<Order> orders =
            await _orderRepository.GetAllAsync(
                userId,
                storeId,
                status,
                cancellationToken);

        return orders
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<OrderResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Order order = await GetExistingOrderAsync(
            id,
            tracked: false,
            cancellationToken);

        EnsureOrderAccess(order);

        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateAsync(
        int id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureAdmin();
        ValidateId(id);

        if (!Enum.IsDefined(request.Status))
        {
            throw new ArgumentException(
                "O status informado é inválido.");
        }

        Order order = await GetExistingOrderAsync(
            id,
            tracked: true,
            cancellationToken);

        ValidateStatusTransition(
            order.Status,
            request.Status);

        OrderStatus previousStatus = order.Status;
        DateTime utcNow = DateTime.UtcNow;

        order.Status = request.Status;
        order.UpdatedAt = utcNow;

        if (request.Status == OrderStatus.Confirmed)
        {
            order.ConfirmedAt = utcNow;
        }

        if (request.Status == OrderStatus.Delivered)
        {
            order.DeliveredAt = utcNow;
        }

        if (request.Status == OrderStatus.Cancelled &&
            previousStatus != OrderStatus.Cancelled)
        {
            ReleaseDeliverySchedule(
                order.DeliverySchedule,
                utcNow);
        }

        _orderRepository.Update(order);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        Order? updatedOrder =
            await _orderRepository.GetByIdAsync(
                order.Id,
                cancellationToken);

        if (updatedOrder is null)
        {
            throw new InvalidOperationException(
                "O pedido foi atualizado, mas não pôde ser carregado.");
        }

        return MapToResponse(updatedOrder);
    }

    public async Task CancelAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Order order = await GetExistingOrderAsync(
            id,
            tracked: true,
            cancellationToken);

        EnsureOrderAccess(order);

        if (order.Status is not
            (OrderStatus.Pending or OrderStatus.Confirmed))
        {
            throw new InvalidOperationException(
                "O pedido não pode ser cancelado no status atual.");
        }

        DateTime utcNow = DateTime.UtcNow;

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = utcNow;

        ReleaseDeliverySchedule(
            order.DeliverySchedule,
            utcNow);

        _orderRepository.Update(order);

        await _orderRepository.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Order> GetExistingOrderAsync(
        int id,
        bool tracked,
        CancellationToken cancellationToken)
    {
        Order? order;

        if (tracked)
        {
            order = await _orderRepository
                .GetTrackedByIdAsync(
                    id,
                    cancellationToken);
        }
        else
        {
            order = await _orderRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);
        }

        return order
            ?? throw new KeyNotFoundException(
                "Pedido não encontrado.");
    }

    private void EnsureOrderAccess(Order order)
    {
        if (_currentUserService.IsAdmin)
        {
            return;
        }

        if (order.UserId != _currentUserService.UserId)
        {
            throw new UnauthorizedAccessException(
                "Você não possui acesso a esse pedido.");
        }
    }

    private void EnsureAdmin()
    {
        if (!_currentUserService.IsAdmin)
        {
            throw new UnauthorizedAccessException(
                "A operação exige perfil de administrador.");
        }
    }

    private static void ValidateCreateRequest(
        CreateOrderRequest request)
    {
        if (request.StoreId <= 0)
        {
            throw new ArgumentException(
                "Informe um mercado válido.");
        }

        if (request.AddressId <= 0)
        {
            throw new ArgumentException(
                "Informe um endereço válido.");
        }

        if (request.DeliveryScheduleId <= 0)
        {
            throw new ArgumentException(
                "Informe um horário de entrega válido.");
        }

        if (!Enum.IsDefined(request.PaymentMethod))
        {
            throw new ArgumentException(
                "A forma de pagamento informada é inválida.");
        }

        if (request.Items is null ||
            request.Items.Count == 0)
        {
            throw new ArgumentException(
                "O pedido deve possuir pelo menos um produto.");
        }

        if (request.Items.Any(
                item =>
                    item.ProductId <= 0 ||
                    item.Quantity <= 0))
        {
            throw new ArgumentException(
                "Os produtos e quantidades do pedido são inválidos.");
        }
    }

    private static void ValidateDeliverySchedule(
        DeliverySchedule? schedule)
    {
        if (schedule is null)
        {
            throw new KeyNotFoundException(
                "Horário de entrega não encontrado ou inativo.");
        }

        DateTime utcNow = DateTime.UtcNow;

        DateTime scheduleEndDateTime =
            schedule.DeliveryDate.Date
                .Add(schedule.EndTime);

        if (scheduleEndDateTime <= utcNow)
        {
            throw new InvalidOperationException(
                "O horário de entrega selecionado já passou.");
        }

        if (schedule.MaximumOrders <= 0)
        {
            throw new InvalidOperationException(
                "O horário de entrega não aceita pedidos.");
        }

        if (schedule.ReservedOrders >=
            schedule.MaximumOrders)
        {
            throw new InvalidOperationException(
                "O horário de entrega selecionado não possui mais vagas.");
        }
    }

    private static void ValidateStatusTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            throw new InvalidOperationException(
                "O pedido já possui o status informado.");
        }

        bool transitionIsValid = currentStatus switch
        {
            OrderStatus.Pending =>
                newStatus is
                    OrderStatus.Confirmed or
                    OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                newStatus is
                    OrderStatus.Shopping or
                    OrderStatus.Cancelled,

            OrderStatus.Shopping =>
                newStatus ==
                OrderStatus.OutForDelivery,

            OrderStatus.OutForDelivery =>
                newStatus ==
                OrderStatus.Delivered,

            OrderStatus.Delivered => false,

            OrderStatus.Cancelled => false,

            _ => false
        };

        if (!transitionIsValid)
        {
            throw new InvalidOperationException(
                $"Não é possível alterar o pedido de " +
                $"{currentStatus} para {newStatus}.");
        }
    }

    private static void ReleaseDeliverySchedule(
        DeliverySchedule schedule,
        DateTime updatedAt)
    {
        if (schedule.ReservedOrders > 0)
        {
            schedule.ReservedOrders--;
        }

        schedule.UpdatedAt = updatedAt;
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "O identificador do pedido é inválido.");
        }
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static OrderResponse MapToResponse(
        Order order)
    {
        decimal productsAmount = order.Items.Sum(
            item => item.Subtotal);

        return new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            CustomerName = order.User.Name,
            StoreId = order.StoreId,
            StoreName = order.Store.Name,
            AddressId = order.AddressId,
            DeliveryAddress = BuildAddress(
                order.Address),
            DeliveryScheduleId =
                order.DeliveryScheduleId,
            DeliveryDate =
                order.DeliverySchedule.DeliveryDate,
            DeliveryStartTime =
                order.DeliverySchedule.StartTime,
            DeliveryEndTime =
                order.DeliverySchedule.EndTime,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus,
            ProductsAmount = productsAmount,
            DeliveryFee = order.DeliveryFee,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            ConfirmedAt = order.ConfirmedAt,
            DeliveredAt = order.DeliveredAt,
            IsActive = order.IsActive,
            Items = order.Items
                .OrderBy(item => item.Id)
                .Select(item => new OrderItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Subtotal = item.Subtotal
                })
                .ToList()
        };
    }

    private static string BuildAddress(
        Address address)
    {
        string streetAndNumber =
            $"{address.Street}, {address.Number}";

        string complement =
            string.IsNullOrWhiteSpace(address.Complement)
                ? string.Empty
                : $" - {address.Complement}";

        return
            $"{streetAndNumber}{complement}, " +
            $"{address.Neighborhood}, " +
            $"{address.City}/{address.State}, " +
            $"CEP {address.ZipCode}";
    }
}