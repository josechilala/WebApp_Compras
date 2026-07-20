using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Data.Configurations;

public sealed class ProductConfiguration :
    IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(product => product.Description)
            .HasMaxLength(500);

        builder.Property(product => product.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(product => product.ImageUrl)
            .HasMaxLength(300);

        builder.Property(product => product.IsAvailable)
            .IsRequired();

        builder.Property(product => product.IsActive)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.HasOne(product => product.Store)
            .WithMany(store => store.Products)
            .HasForeignKey(product => product.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(product => product.OrderItems)
            .WithOne(orderItem => orderItem.Product)
            .HasForeignKey(orderItem => orderItem.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(product => new
        {
            product.StoreId,
            product.Name
        });
    }
}