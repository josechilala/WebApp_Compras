using Microsoft.EntityFrameworkCore;
using WebAPP_Compras.Data;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Repositories.Interfaces;

namespace WebAPP_Compras.Repositories;

public sealed class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Address>> GetAllByUserAsync(
        int userId,
        bool includeInactive,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Address> query = _context.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == userId);

        if (!includeInactive)
        {
            query = query.Where(address => address.IsActive);
        }

        return await query
            .OrderByDescending(address => address.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Address?> GetByIdForUserAsync(
        int addressId,
        int userId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Address> query = _context.Addresses
            .AsNoTracking()
            .Where(address =>
                address.Id == addressId &&
                address.UserId == userId);

        if (!includeInactive)
        {
            query = query.Where(address => address.IsActive);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Address?> GetTrackedByIdForUserAsync(
        int addressId,
        int userId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Address> query = _context.Addresses
            .Where(address =>
                address.Id == addressId &&
                address.UserId == userId);

        if (!includeInactive)
        {
            query = query.Where(address => address.IsActive);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Address?> GetActiveByIdForUserAsync(
        int addressId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Addresses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                address =>
                    address.Id == addressId &&
                    address.UserId == userId &&
                    address.IsActive,
                cancellationToken);
    }

    public async Task AddAsync(
        Address address,
        CancellationToken cancellationToken = default)
    {
        await _context.Addresses.AddAsync(
            address,
            cancellationToken);
    }

    public void Update(Address address)
    {
        _context.Addresses.Update(address);
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}