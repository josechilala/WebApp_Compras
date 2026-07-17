using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);

    DateTime GetExpiration();
}