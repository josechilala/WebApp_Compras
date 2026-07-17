using Microsoft.EntityFrameworkCore;
using WebAPP_Compras.Data;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;

namespace WebAPP_Compras.Repositories;

public sealed class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _context;

    public StoreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Store>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Store> query = _context.Stores
            .AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(store => store.IsActive);
        }

        return await query
            .OrderBy(store => store.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Store?> GetByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Store> query = _context.Stores;

        if (!includeInactive)
        {
            query = query.Where(store => store.IsActive);
        }

        return await query.FirstOrDefaultAsync(
            store => store.Id == id,
            cancellationToken);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        int? ignoredStoreId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name
            .Trim()
            .ToLowerInvariant();

        return await _context.Stores.AnyAsync(
            store =>
                store.Name.ToLower() == normalizedName &&
                (!ignoredStoreId.HasValue ||
                 store.Id != ignoredStoreId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Store store,
        CancellationToken cancellationToken = default)
    {
        await _context.Stores.AddAsync(
            store,
            cancellationToken);
    }

    public void Update(Store store)
    {
        _context.Stores.Update(store);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}