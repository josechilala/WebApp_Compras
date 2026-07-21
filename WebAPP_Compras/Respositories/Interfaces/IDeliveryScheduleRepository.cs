using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Repositories.Interfaces;

public interface IDeliveryScheduleRepository
{
    Task<IReadOnlyCollection<DeliverySchedule>> GetAllAsync(
        bool includeInactive,
        bool onlyFuture,
        CancellationToken cancellationToken = default);

    Task<DeliverySchedule?> GetByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<DeliverySchedule?> GetTrackedByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<DeliverySchedule?> GetAvailableTrackedByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingScheduleAsync(
        DateTime deliveryDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int? ignoredScheduleId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        DeliverySchedule deliverySchedule,
        CancellationToken cancellationToken = default);

    void Update(DeliverySchedule deliverySchedule);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}