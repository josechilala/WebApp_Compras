using WebAPP_Compras.Models.DTOs.Stores;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public sealed class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;

    public StoreService(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<IReadOnlyCollection<StoreResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Store> stores =
            await _storeRepository.GetAllAsync(
                includeInactive,
                cancellationToken);

        return stores
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<StoreResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        Store store = await GetExistingStoreAsync(
            id,
            includeInactive: false,
            cancellationToken);

        return MapToResponse(store);
    }

    public async Task<StoreResponse> CreateAsync(
        CreateStoreRequest request,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = NormalizeRequiredText(
            request.Name,
            "nome");

        bool nameExists =
            await _storeRepository.NameExistsAsync(
                normalizedName,
                cancellationToken: cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Já existe um mercado cadastrado com esse nome.");
        }

        var store = new Store
        {
            Name = normalizedName,
            Address = NormalizeRequiredText(
                request.Address,
                "endereço"),
            Phone = NormalizeOptionalText(request.Phone),
            ImageUrl = NormalizeOptionalText(request.ImageUrl),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _storeRepository.AddAsync(
            store,
            cancellationToken);

        await _storeRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(store);
    }

    public async Task<StoreResponse> UpdateAsync(
        int id,
        UpdateStoreRequest request,
        CancellationToken cancellationToken = default)
    {
        Store store = await GetExistingStoreAsync(
            id,
            includeInactive: true,
            cancellationToken);

        string normalizedName = NormalizeRequiredText(
            request.Name,
            "nome");

        bool nameExists =
            await _storeRepository.NameExistsAsync(
                normalizedName,
                ignoredStoreId: id,
                cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Já existe outro mercado cadastrado com esse nome.");
        }

        store.Name = normalizedName;
        store.Address = NormalizeRequiredText(
            request.Address,
            "endereço");
        store.Phone = NormalizeOptionalText(request.Phone);
        store.ImageUrl = NormalizeOptionalText(request.ImageUrl);
        store.IsActive = request.IsActive;
        store.UpdatedAt = DateTime.UtcNow;

        _storeRepository.Update(store);

        await _storeRepository.SaveChangesAsync(
            cancellationToken);

        return MapToResponse(store);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        Store store = await GetExistingStoreAsync(
            id,
            includeInactive: true,
            cancellationToken);

        if (!store.IsActive)
        {
            return;
        }

        store.IsActive = false;
        store.UpdatedAt = DateTime.UtcNow;

        _storeRepository.Update(store);

        await _storeRepository.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Store> GetExistingStoreAsync(
        int id,
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "O identificador do mercado é inválido.");
        }

        Store? store = await _storeRepository.GetByIdAsync(
            id,
            includeInactive,
            cancellationToken);

        return store
            ?? throw new KeyNotFoundException(
                "Mercado não encontrado.");
    }

    private static StoreResponse MapToResponse(Store store)
    {
        return new StoreResponse
        {
            Id = store.Id,
            Name = store.Name,
            Address = store.Address,
            Phone = store.Phone,
            ImageUrl = store.ImageUrl,
            IsActive = store.IsActive,
            CreatedAt = store.CreatedAt,
            UpdatedAt = store.UpdatedAt
        };
    }

    private static string NormalizeRequiredText(
        string value,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"O campo {fieldName} é obrigatório.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalText(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}