using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(address => address.Id);

        builder.Property(address => address.Street)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(address => address.Number)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(address => address.Complement)
            .HasMaxLength(100);

        builder.Property(address => address.Neighborhood)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.State)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(address => address.ZipCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(address => address.ReferencePoint)
            .HasMaxLength(150);

        builder.Property(address => address.CreatedAt)
            .IsRequired();

        builder.Property(address => address.IsActive)
            .IsRequired();

        builder.HasOne(address => address.User)
            .WithMany(user => user.Addresses)
            .HasForeignKey(address => address.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(address => address.Orders)
            .WithOne(order => order.Address)
            .HasForeignKey(order => order.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}