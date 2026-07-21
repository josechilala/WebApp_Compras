namespace WebAPP_Compras.Models.DTOs.Addresses;

public sealed class AddressResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Street { get; set; } = string.Empty;

    public string Number { get; set; } = string.Empty;

    public string? Complement { get; set; }

    public string Neighborhood { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public string? ReferencePoint { get; set; }

    public string FullAddress { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}