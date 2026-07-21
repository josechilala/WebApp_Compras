namespace WebAPP_Compras.Services.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    int UserId { get; }

    bool IsAdmin { get; }
}