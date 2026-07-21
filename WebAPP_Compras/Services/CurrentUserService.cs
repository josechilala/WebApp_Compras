using System.Security.Claims;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public int UserId
    {
        get
        {
            string? userIdClaim = User?.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    userIdClaim,
                    out int userId))
            {
                throw new UnauthorizedAccessException(
                    "Não foi possível identificar o usuário autenticado.");
            }

            return userId;
        }
    }

    public bool IsAdmin =>
        User?.IsInRole("Admin") == true;
}