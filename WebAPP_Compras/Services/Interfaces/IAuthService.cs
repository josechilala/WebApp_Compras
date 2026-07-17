using WebAPP_Compras.Models.DTOs.Auth;

namespace WebAPP_Compras.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request);

    Task<LoginResponse> LoginAsync(
        LoginRequest request);
}