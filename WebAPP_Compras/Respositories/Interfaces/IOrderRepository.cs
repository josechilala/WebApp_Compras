using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<Order>> GetAllAsync(
        int? userId,
        int? storeId,
        OrderStatus? status,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Order?> GetTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);

    void Update(Order order);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}