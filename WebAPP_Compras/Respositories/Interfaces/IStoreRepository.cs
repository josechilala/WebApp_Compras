using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Repositories.Interfaces;

public interface IStoreRepository
{
    Task<IReadOnlyCollection<Store>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<Store?> GetByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsAsync(
        string name,
        int? ignoredStoreId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Store store,
        CancellationToken cancellationToken = default);

    void Update(Store store);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}