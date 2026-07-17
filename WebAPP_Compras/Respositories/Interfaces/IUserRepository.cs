using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);

    Task<User?> GetByEmailAsync(string email);

    Task<User> AddAsync(User user);
}