using System.ComponentModel.DataAnnotations;
using WebAPP_Compras.Models.Validations;

namespace WebAPP_Compras.Models.DTOs.Products;

public sealed class UpdateProductRequest
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Informe um mercado válido.")]
    public int StoreId { get; set; }

    [Required(
        ErrorMessage = "O nome do produto é obrigatório.")]
    [MaxLength(
        150,
        ErrorMessage =
            "O nome deve possuir no máximo 150 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(
        500,
        ErrorMessage =
            "A descrição deve possuir no máximo 500 caracteres.")]
    public string? Description { get; set; }

    [PositiveDecimal(
        ErrorMessage =
            "O preço do produto deve ser maior que zero.")]
    public decimal Price { get; set; }

    [MaxLength(
        300,
        ErrorMessage =
            "A URL deve possuir no máximo 300 caracteres.")]
    [Url(
        ErrorMessage =
            "Informe uma URL de imagem válida.")]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public bool IsActive { get; set; } = true;
}