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
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        PasswordHasher<User> passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request)
    {
        string normalizedEmail = request.Email
            .Trim()
            .ToLowerInvariant();

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

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request)
    {
        string normalizedEmail = request.Email
            .Trim()
            .ToLowerInvariant();

        User? user = await _userRepository
            .GetByEmailAsync(normalizedEmail);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "E-mail ou senha inválidos.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException(
                "Este usuário está inativo.");
        }

        PasswordVerificationResult passwordResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "E-mail ou senha inválidos.");
        }

        string token = _jwtService.GenerateToken(user);
        DateTime expiresAt = _jwtService.GetExpiration();

        return new LoginResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token,
            ExpiresAt = expiresAt,
            Message = "Login realizado com sucesso."
        };
    }
}