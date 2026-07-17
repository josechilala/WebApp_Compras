using System.ComponentModel.DataAnnotations;
using WebAPP_Compras.Models.Common;

namespace WebAPP_Compras.Models.Entities;

public class Address : BaseEntity
{
    public int UserId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Complement { get; set; }

    [Required]
    [MaxLength(100)]
    public string Neighborhood { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(2)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? ReferencePoint { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}