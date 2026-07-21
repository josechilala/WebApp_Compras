using Microsoft.EntityFrameworkCore;
using WebAPP_Compras.Data;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;

namespace WebAPP_Compras.Repositories;

public sealed class DeliveryScheduleRepository
    : IDeliveryScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public DeliveryScheduleRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<DeliverySchedule>> GetAllAsync(
        bool includeInactive,
        bool onlyFuture,
        CancellationToken cancellationToken = default)
    {
        IQueryable<DeliverySchedule> query =
            _context.DeliverySchedules
                .AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(schedule => schedule.IsActive);
        }

        if (onlyFuture)
        {
            DateTime today = DateTime.UtcNow.Date;

            query = query.Where(
                schedule =>
                    schedule.DeliveryDate.Date >= today);
        }

        return await query
            .OrderBy(schedule => schedule.DeliveryDate)
            .ThenBy(schedule => schedule.StartTime)
            .ToListAsync(cancellationToken);
    }

    public Task<DeliverySchedule?> GetByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<DeliverySchedule> query =
            _context.DeliverySchedules
                .AsNoTracking()
                .Where(schedule => schedule.Id == id);

        if (!includeInactive)
        {
            query = query.Where(schedule => schedule.IsActive);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<DeliverySchedule?> GetTrackedByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<DeliverySchedule> query =
            _context.DeliverySchedules
                .Where(schedule => schedule.Id == id);

        if (!includeInactive)
        {
            query = query.Where(schedule => schedule.IsActive);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<DeliverySchedule?> GetAvailableTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.DeliverySchedules
            .FirstOrDefaultAsync(
                schedule =>
                    schedule.Id == id &&
                    schedule.IsActive &&
                    schedule.ReservedOrders <
                    schedule.MaximumOrders,
                cancellationToken);
    }

    public Task<bool> HasOverlappingScheduleAsync(
        DateTime deliveryDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int? ignoredScheduleId = null,
        CancellationToken cancellationToken = default)
    {
        DateTime normalizedDate = deliveryDate.Date;

        return _context.DeliverySchedules.AnyAsync(
            schedule =>
                schedule.IsActive &&
                schedule.DeliveryDate.Date == normalizedDate &&
                schedule.StartTime < endTime &&
                startTime < schedule.EndTime &&
                (!ignoredScheduleId.HasValue ||
                 schedule.Id != ignoredScheduleId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        DeliverySchedule deliverySchedule,
        CancellationToken cancellationToken = default)
    {
        await _context.DeliverySchedules.AddAsync(
            deliverySchedule,
            cancellationToken);
    }

    public void Update(DeliverySchedule deliverySchedule)
    {
        _context.DeliverySchedules.Update(
            deliverySchedule);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}