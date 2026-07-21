using WebAPP_Compras.Models.DTOs.Addresses;

namespace WebAPP_Compras.Services.Interfaces;

public interface IAddressService
{
    Task<AddressResponse> CreateAsync(
        CreateAddressRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AddressResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<AddressResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AddressResponse> UpdateAsync(
        int id,
        UpdateAddressRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}