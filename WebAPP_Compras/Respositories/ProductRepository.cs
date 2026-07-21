using Microsoft.EntityFrameworkCore;
using WebAPP_Compras.Data;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;

namespace WebAPP_Compras.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(
        int? storeId,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products
            .AsNoTracking()
            .Include(product => product.Store);

        if (storeId.HasValue)
        {
            query = query.Where(
                product => product.StoreId == storeId.Value);
        }

        if (!includeInactive)
        {
            query = query.Where(
                product =>
                    product.IsActive &&
                    product.Store.IsActive);
        }

        return await query
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products
            .AsNoTracking()
            .Include(product => product.Store);

        if (!includeInactive)
        {
            query = query.Where(
                product =>
                    product.IsActive &&
                    product.Store.IsActive);
        }

        return query.FirstOrDefaultAsync(
            product => product.Id == id,
            cancellationToken);
    }

    public Task<Product?> GetTrackedByIdAsync(
        int id,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products
            .Include(product => product.Store);

        if (!includeInactive)
        {
            query = query.Where(
                product =>
                    product.IsActive &&
                    product.Store.IsActive);
        }

        return query.FirstOrDefaultAsync(
            product => product.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>>
        GetAvailableByIdsAsync(
            IReadOnlyCollection<int> productIds,
            int storeId,
            CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(product => product.Store)
            .Where(
                product =>
                    productIds.Contains(product.Id) &&
                    product.StoreId == storeId &&
                    product.IsActive &&
                    product.IsAvailable &&
                    product.Store.IsActive)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> NameExistsInStoreAsync(
        int storeId,
        string name,
        int? ignoredProductId = null,
        CancellationToken cancellationToken = default)
    {
        string normalizedName = name
            .Trim()
            .ToLower();

        return _context.Products.AnyAsync(
            product =>
                product.StoreId == storeId &&
                product.Name.ToLower() == normalizedName &&
                (!ignoredProductId.HasValue ||
                 product.Id != ignoredProductId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(
            product,
            cancellationToken);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}