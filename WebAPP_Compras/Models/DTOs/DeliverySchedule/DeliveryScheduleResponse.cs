namespace WebAPP_Compras.Models.DTOs.DeliverySchedules;

public sealed class DeliveryScheduleResponse
{
    public int Id { get; set; }

    public DateTime DeliveryDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int MaximumOrders { get; set; }

    public int ReservedOrders { get; set; }

    public int AvailableOrders { get; set; }

    public bool HasAvailability { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}