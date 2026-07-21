using Microsoft.EntityFrameworkCore;
using WebAPP_Compras.Data;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Models.Enums;
using WebAPP_Compras.Repositories.Interfaces;

namespace WebAPP_Compras.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Order>> GetAllAsync(
        int? userId,
        int? storeId,
        OrderStatus? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = BuildQuery()
            .AsNoTracking();

        if (userId.HasValue)
        {
            query = query.Where(order => order.UserId == userId.Value);
        }

        if (storeId.HasValue)
        {
            query = query.Where(order => order.StoreId == storeId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(order => order.Status == status.Value);
        }

        return await query
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await BuildQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);
    }

    public async Task<Order?> GetTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await BuildQuery()
            .FirstOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(
            order,
            cancellationToken);
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Order> BuildQuery()
    {
        return _context.Orders
            .Include(order => order.User)
            .Include(order => order.Store)
            .Include(order => order.Address)
            .Include(order => order.DeliverySchedule)
            .Include(order => order.Items)
                .ThenInclude(item => item.Product);
    }
}