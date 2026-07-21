using WebAPP_Compras.Models.DTOs.DeliverySchedules;

namespace WebAPP_Compras.Services.Interfaces;

public interface IDeliveryScheduleService
{
    Task<DeliveryScheduleResponse> CreateAsync(
        CreateDeliveryScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<DeliveryScheduleResponse>> GetAllAsync(
        bool includeInactive,
        bool onlyFuture,
        CancellationToken cancellationToken = default);

    Task<DeliveryScheduleResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<DeliveryScheduleResponse> UpdateAsync(
        int id,
        UpdateDeliveryScheduleRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}