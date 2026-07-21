using System.ComponentModel.DataAnnotations;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Models.DTOs.Orders;

public sealed class UpdateOrderRequest
{
    [EnumDataType(
        typeof(OrderStatus),
        ErrorMessage = "Informe um status de pedido válido.")]
    public OrderStatus Status { get; set; }
}