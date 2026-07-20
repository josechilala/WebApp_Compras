using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> GetAllAsync(
        int? storeId,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<Product?> GetTrackedByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<bool> NameExistsInStoreAsync(
        int storeId,
        string name,
        int? ignoredProductId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    void Update(Product product);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}