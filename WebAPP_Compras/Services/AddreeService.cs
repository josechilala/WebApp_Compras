using WebAPP_Compras.Models.DTOs.Addresses;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public sealed class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddressService(
        IAddressRepository addressRepository,
        ICurrentUserService currentUserService)
    {
        _addressRepository = addressRepository;
        _currentUserService = currentUserService;
    }

    public async Task<AddressResponse> CreateAsync(
        CreateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateAddress(
            request.Street,
            request.Number,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode);

        var address = new Address
        {
            UserId = _currentUserService.UserId,
            Street = request.Street.Trim(),
            Number = request.Number.Trim(),
            Complement = NormalizeOptionalText(request.Complement),
            Neighborhood = request.Neighborhood.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim().ToUpperInvariant(),
            ZipCode = NormalizeZipCode(request.ZipCode),
            ReferencePoint = NormalizeOptionalText(
                request.ReferencePoint),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _addressRepository.AddAsync(
            address,
            cancellationToken);

        await _addressRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(address);
    }

    public async Task<IReadOnlyCollection<AddressResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Address> addresses =
            await _addressRepository.GetAllByUserAsync(
                _currentUserService.UserId,
                includeInactive,
                cancellationToken);

        return addresses
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AddressResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Address? address =
            await _addressRepository.GetByIdForUserAsync(
                id,
                _currentUserService.UserId,
                includeInactive: true,
                cancellationToken);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Endereço não encontrado.");
        }

        return MapToResponse(address);
    }

    public async Task<AddressResponse> UpdateAsync(
        int id,
        UpdateAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        ValidateAddress(
            request.Street,
            request.Number,
            request.Neighborhood,
            request.City,
            request.State,
            request.ZipCode);

        Address? address =
            await _addressRepository.GetTrackedByIdForUserAsync(
                id,
                _currentUserService.UserId,
                includeInactive: false,
                cancellationToken);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Endereço não encontrado ou inativo.");
        }

        address.Street = request.Street.Trim();
        address.Number = request.Number.Trim();
        address.Complement = NormalizeOptionalText(
            request.Complement);
        address.Neighborhood = request.Neighborhood.Trim();
        address.City = request.City.Trim();
        address.State = request.State.Trim().ToUpperInvariant();
        address.ZipCode = NormalizeZipCode(request.ZipCode);
        address.ReferencePoint = NormalizeOptionalText(
            request.ReferencePoint);
        address.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(address);

        await _addressRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(address);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        Address? address =
            await _addressRepository.GetTrackedByIdForUserAsync(
                id,
                _currentUserService.UserId,
                includeInactive: false,
                cancellationToken);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Endereço não encontrado ou já inativo.");
        }

        address.IsActive = false;
        address.UpdatedAt = DateTime.UtcNow;

        _addressRepository.Update(address);

        await _addressRepository.SaveChangesAsync(
            cancellationToken);
    }

    private static void ValidateAddress(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new ArgumentException(
                "A rua é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(number))
        {
            throw new ArgumentException(
                "O número é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(neighborhood))
        {
            throw new ArgumentException(
                "O bairro é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException(
                "A cidade é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(state) ||
            state.Trim().Length != 2)
        {
            throw new ArgumentException(
                "O estado deve possuir exatamente 2 caracteres.");
        }

        string normalizedZipCode = NormalizeZipCode(zipCode);

        if (normalizedZipCode.Length != 8 ||
            !normalizedZipCode.All(char.IsDigit))
        {
            throw new ArgumentException(
                "O CEP deve possuir exatamente 8 números.");
        }
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "O identificador do endereço é inválido.");
        }
    }

    private static string NormalizeZipCode(string zipCode)
    {
        return zipCode
            .Trim()
            .Replace("-", string.Empty)
            .Replace(".", string.Empty);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static AddressResponse MapToResponse(
        Address address)
    {
        return new AddressResponse
        {
            Id = address.Id,
            UserId = address.UserId,
            Street = address.Street,
            Number = address.Number,
            Complement = address.Complement,
            Neighborhood = address.Neighborhood,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            ReferencePoint = address.ReferencePoint,
            FullAddress = BuildFullAddress(address),
            CreatedAt = address.CreatedAt,
            UpdatedAt = address.UpdatedAt,
            IsActive = address.IsActive
        };
    }

    private static string BuildFullAddress(Address address)
    {
        string complement =
            string.IsNullOrWhiteSpace(address.Complement)
                ? string.Empty
                : $" - {address.Complement}";

        return
            $"{address.Street}, {address.Number}" +
            $"{complement}, " +
            $"{address.Neighborhood}, " +
            $"{address.City}/{address.State}, " +
            $"CEP {address.ZipCode}";
    }
}