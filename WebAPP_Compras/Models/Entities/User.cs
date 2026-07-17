using System.ComponentModel.DataAnnotations;
using WebAPP_Compras.Models.Common;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Models.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public UserRole Role { get; set; } = UserRole.Customer;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}