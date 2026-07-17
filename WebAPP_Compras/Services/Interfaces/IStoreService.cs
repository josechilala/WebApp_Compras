using WebAPP_Compras.Models.DTOs.Stores;

namespace WebAPP_Compras.Services.Interfaces;

public interface IStoreService
{
    Task<IReadOnlyCollection<StoreResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<StoreResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<StoreResponse> CreateAsync(
        CreateStoreRequest request,
        CancellationToken cancellationToken = default);

    Task<StoreResponse> UpdateAsync(
        int id,
        UpdateStoreRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}