using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Models.DTOs.Orders;

public sealed class OrderResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int StoreId { get; set; }

    public string StoreName { get; set; } = string.Empty;

    public int AddressId { get; set; }

    public string DeliveryAddress { get; set; } = string.Empty;

    public int DeliveryScheduleId { get; set; }

    public DateTime DeliveryDate { get; set; }

    public TimeSpan DeliveryStartTime { get; set; }

    public TimeSpan DeliveryEndTime { get; set; }

    public OrderStatus Status { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public decimal ProductsAmount { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public bool IsActive { get; set; }

    public IReadOnlyCollection<OrderItemResponse> Items { get; set; } =
        Array.Empty<OrderItemResponse>();
}