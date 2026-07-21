using System.ComponentModel.DataAnnotations;
using WebAPP_Compras.Models.Enums;

namespace WebAPP_Compras.Models.DTOs.Orders;

public sealed class CreateOrderRequest
{
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Informe um mercado válido.")]
    public int StoreId { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Informe um endereço válido.")]
    public int AddressId { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Informe um horário de entrega válido.")]
    public int DeliveryScheduleId { get; set; }

    [EnumDataType(
        typeof(PaymentMethod),
        ErrorMessage = "Informe uma forma de pagamento válida.")]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(
        500,
        ErrorMessage = "As observações devem possuir no máximo 500 caracteres.")]
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Informe os produtos do pedido.")]
    [MinLength(
        1,
        ErrorMessage = "O pedido deve possuir pelo menos um produto.")]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}