using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPP_Compras.Models.Common;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Models.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }

    public PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }

    public Order Order { get; set; } = null!;
}