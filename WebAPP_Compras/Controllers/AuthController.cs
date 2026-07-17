using Microsoft.AspNetCore.Mvc;
using WebAPP_Compras.Models.DTOs.Auth;
using WebAPP_Compras.Services.Interfaces;

namespace WebAPP_Compras.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Cadastro de usuário
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            RegisterResponse response =
                await _authService.RegisterAsync(request);

            return Created("", response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "Erro interno do servidor."
            });
        }
    }
}