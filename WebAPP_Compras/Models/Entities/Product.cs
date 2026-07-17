using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPP_Compras.Models.Common;

namespace WebAPP_Compras.Models.Entities;

public class Product : BaseEntity
{
    public int StoreId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [MaxLength(300)]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;

    public Store Store { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}