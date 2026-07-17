using System.ComponentModel.DataAnnotations;
using WebAPP_Compras.Models.Common;

namespace WebAPP_Compras.Models.Entities;

public class Store : BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? ImageUrl { get; set; }

    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}