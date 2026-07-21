using WebAPP_Compras.Models.DTOs.Orders;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OrderResponse>> GetMyOrdersAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(
        int? userId,
        int? storeId,
        OrderStatus? status,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<OrderResponse> UpdateAsync(
        int id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        int id,
        CancellationToken cancellationToken = default);
}