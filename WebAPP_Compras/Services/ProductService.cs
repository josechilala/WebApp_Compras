using WebAPP_Compras.Models.DTOs.Products;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IStoreRepository _storeRepository;

    public ProductService(
        IProductRepository productRepository,
        IStoreRepository storeRepository)
    {
        _productRepository = productRepository;
        _storeRepository = storeRepository;
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        int? storeId,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        if (storeId.HasValue && storeId.Value <= 0)
        {
            throw new ArgumentException(
                "O identificador do mercado é inválido.");
        }

        IReadOnlyCollection<Product> products =
            await _productRepository.GetAllAsync(
                storeId,
                includeInactive,
                cancellationToken);

        return products
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ProductResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Product? product =
            await _productRepository.GetByIdAsync(
                id,
                includeInactive: false,
                cancellationToken: cancellationToken);

        return product is null
            ? throw new KeyNotFoundException(
                "Produto não encontrado.")
            : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        Store store = await GetActiveStoreAsync(
            request.StoreId,
            cancellationToken);

        string normalizedName =
            NormalizeRequiredText(request.Name, "nome");

        bool nameExists =
            await _productRepository.NameExistsInStoreAsync(
                request.StoreId,
                normalizedName,
                cancellationToken: cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Já existe um produto com esse nome neste mercado.");
        }

        ValidatePrice(request.Price);

        var product = new Product
        {
            StoreId = store.Id,
            Name = normalizedName,
            Description = NormalizeOptionalText(
                request.Description),
            Price = request.Price,
            ImageUrl = NormalizeOptionalText(
                request.ImageUrl),
            IsAvailable = request.IsAvailable,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Store = store
        };

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Product? product =
            await _productRepository.GetTrackedByIdAsync(
                id,
                includeInactive: true,
                cancellationToken: cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Produto não encontrado.");
        }

        Store store = await GetActiveStoreAsync(
            request.StoreId,
            cancellationToken);

        string normalizedName =
            NormalizeRequiredText(request.Name, "nome");

        bool nameExists =
            await _productRepository.NameExistsInStoreAsync(
                request.StoreId,
                normalizedName,
                ignoredProductId: id,
                cancellationToken: cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Já existe outro produto com esse nome neste mercado.");
        }

        ValidatePrice(request.Price);

        product.StoreId = store.Id;
        product.Store = store;
        product.Name = normalizedName;
        product.Description = NormalizeOptionalText(
            request.Description);
        product.Price = request.Price;
        product.ImageUrl = NormalizeOptionalText(
            request.ImageUrl);
        product.IsAvailable = request.IsAvailable;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        _productRepository.Update(product);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(product);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Product? product =
            await _productRepository.GetTrackedByIdAsync(
                id,
                includeInactive: true,
                cancellationToken: cancellationToken);

        if (product is null)
        {
            throw new KeyNotFoundException(
                "Produto não encontrado.");
        }

        if (!product.IsActive)
        {
            return;
        }

        product.IsActive = false;
        product.IsAvailable = false;
        product.UpdatedAt = DateTime.UtcNow;

        _productRepository.Update(product);

        await _productRepository.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Store> GetActiveStoreAsync(
        int storeId,
        CancellationToken cancellationToken)
    {
        if (storeId <= 0)
        {
            throw new ArgumentException(
                "O identificador do mercado é inválido.");
        }

        Store? store = await _storeRepository.GetByIdAsync(
            storeId,
            includeInactive: false,
            cancellationToken: cancellationToken);

        return store
            ?? throw new KeyNotFoundException(
                "Mercado não encontrado ou inativo.");
    }

    private static ProductResponse MapToResponse(
        Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            StoreId = product.StoreId,
            StoreName = product.Store?.Name ?? string.Empty,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "O identificador do produto é inválido.");
        }
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentException(
                "O preço do produto deve ser maior que zero.");
        }
    }

    private static string NormalizeRequiredText(
        string value,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"O campo {fieldName} é obrigatório.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}