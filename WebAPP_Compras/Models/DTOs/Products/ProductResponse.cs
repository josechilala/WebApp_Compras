namespace WebAPP_Compras.Models.DTOs.Products;

public sealed class ProductResponse
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public string StoreName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}