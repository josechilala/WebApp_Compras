using WebAPP_Compras.Models.Common;

namespace WebAPP_Compras.Models.Entities;

public class DeliverySchedule : BaseEntity
{
    public DateTime DeliveryDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int MaximumOrders { get; set; }

    public int ReservedOrders { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}