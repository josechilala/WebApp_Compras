using System.ComponentModel.DataAnnotations;

namespace WebAPP_Compras.Models.DTOs.Addresses;

public sealed class UpdateAddressRequest
{
    [Required(ErrorMessage = "A rua é obrigatória.")]
    [StringLength(
        150,
        MinimumLength = 2,
        ErrorMessage = "A rua deve possuir entre 2 e 150 caracteres.")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número é obrigatório.")]
    [StringLength(
        20,
        ErrorMessage = "O número deve possuir no máximo 20 caracteres.")]
    public string Number { get; set; } = string.Empty;

    [StringLength(
        100,
        ErrorMessage = "O complemento deve possuir no máximo 100 caracteres.")]
    public string? Complement { get; set; }

    [Required(ErrorMessage = "O bairro é obrigatório.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O bairro deve possuir entre 2 e 100 caracteres.")]
    public string Neighborhood { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "A cidade deve possuir entre 2 e 100 caracteres.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado é obrigatório.")]
    [StringLength(
        2,
        MinimumLength = 2,
        ErrorMessage = "O estado deve possuir exatamente 2 caracteres.")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [StringLength(
        9,
        MinimumLength = 8,
        ErrorMessage = "O CEP deve possuir entre 8 e 9 caracteres.")]
    public string ZipCode { get; set; } = string.Empty;

    [StringLength(
        200,
        ErrorMessage = "O ponto de referência deve possuir no máximo 200 caracteres.")]
    public string? ReferencePoint { get; set; }
}