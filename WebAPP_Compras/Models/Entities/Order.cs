using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPP_Compras.Models.Common;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Models.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }

    public int StoreId { get; set; }

    public int AddressId { get; set; }

    public int DeliveryScheduleId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DeliveryFee { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public User User { get; set; } = null!;

    public Store Store { get; set; } = null!;

    public Address Address { get; set; } = null!;

    public DeliverySchedule DeliverySchedule { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public Payment? Payment { get; set; }
}