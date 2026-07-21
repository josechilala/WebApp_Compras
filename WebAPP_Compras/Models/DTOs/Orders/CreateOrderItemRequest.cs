using System.ComponentModel.DataAnnotations;

namespace WebAPP_Compras.Models.DTOs.Orders;

public sealed class CreateOrderItemRequest
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Informe um produto válido.")]
    public int ProductId { get; set; }

    [Range(
        1,
        100,
        ErrorMessage = "A quantidade deve estar entre 1 e 100.")]
    public int Quantity { get; set; }
}