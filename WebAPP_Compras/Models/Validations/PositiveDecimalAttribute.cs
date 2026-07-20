using System.ComponentModel.DataAnnotations;

namespace WebAPP_Compras.Models.Validations;

[AttributeUsage(
    AttributeTargets.Property |
    AttributeTargets.Field |
    AttributeTargets.Parameter)]
public sealed class PositiveDecimalAttribute : ValidationAttribute
{
    public PositiveDecimalAttribute()
        : base("O valor deve ser maior que zero.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is decimal decimalValue &&
               decimalValue > 0;
    }
}