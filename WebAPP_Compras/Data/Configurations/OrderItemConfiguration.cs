using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.Quantity)
            .IsRequired();

        builder.Property(orderItem => orderItem.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(orderItem => orderItem.Subtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(orderItem => orderItem.CreatedAt)
            .IsRequired();

        builder.Property(orderItem => orderItem.IsActive)
            .IsRequired();

        builder.HasOne(orderItem => orderItem.Order)
            .WithMany(order => order.Items)
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(orderItem => orderItem.Product)
            .WithMany(product => product.OrderItems)
            .HasForeignKey(orderItem => orderItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}