using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Repositories.Interfaces;

public interface IAddressRepository
{
    Task<IReadOnlyCollection<Address>> GetAllByUserAsync(
        int userId,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<Address?> GetByIdForUserAsync(
        int addressId,
        int userId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<Address?> GetTrackedByIdForUserAsync(
        int addressId,
        int userId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    Task<Address?> GetActiveByIdForUserAsync(
        int addressId,
        int userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Address address,
        CancellationToken cancellationToken = default);

    void Update(Address address);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}