using System.ComponentModel.DataAnnotations;

namespace WebAPP_Compras.Models.DTOs.DeliverySchedules;

public sealed class CreateDeliveryScheduleRequest
{
    [Required(ErrorMessage = "A data de entrega é obrigatória.")]
    public DateTime DeliveryDate { get; set; }

    [Required(ErrorMessage = "O horário inicial é obrigatório.")]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "O horário final é obrigatório.")]
    public TimeSpan EndTime { get; set; }

    [Range(
        1,
        1000,
        ErrorMessage = "A quantidade máxima de pedidos deve estar entre 1 e 1000.")]
    public int MaximumOrders { get; set; }
}