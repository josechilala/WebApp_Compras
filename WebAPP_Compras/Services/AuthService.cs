using Microsoft.AspNetCore.Identity;
using WebAPP_Compras.Models.DTOs.Auth;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Models.Enums;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        PasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request)
    {
        string normalizedEmail = request.Email
            .Trim()
            .ToLower();

        bool emailAlreadyExists =
            await _userRepository.EmailExistsAsync(normalizedEmail);

        if (emailAlreadyExists)
        {
            throw new InvalidOperationException(
                "Já existe um usuário cadastrado com esse e-mail.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            Phone = string.IsNullOrWhiteSpace(request.Phone)
                ? null
                : request.Phone.Trim(),
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            request.Password);

        await _userRepository.AddAsync(user);

        return new RegisterResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Message = "Usuário cadastrado com sucesso."
        };
    }
}