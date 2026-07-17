using System.ComponentModel.DataAnnotations;

namespace WebAPP_Compras.Models.DTOs.Stores;

public sealed class UpdateStoreRequest
{
    [Required(ErrorMessage = "O nome do mercado é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve possuir no máximo 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O endereço do mercado é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O endereço deve possuir no máximo 200 caracteres.")]
    public string Address { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "O telefone deve possuir no máximo 20 caracteres.")]
    public string? Phone { get; set; }

    [MaxLength(300, ErrorMessage = "A URL da imagem deve possuir no máximo 300 caracteres.")]
    [Url(ErrorMessage = "Informe uma URL de imagem válida.")]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}