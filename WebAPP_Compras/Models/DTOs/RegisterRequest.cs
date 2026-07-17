using System.ComponentModel.DataAnnotations;

namespace WebAPP_Compras.Models.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6, ErrorMessage = "A senha deve possuir pelo menos 6 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da senha é obrigatória.")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não são iguais.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }
}