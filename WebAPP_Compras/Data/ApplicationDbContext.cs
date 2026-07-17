using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebAPP_Compras.Models.Entities;

namespace WebAPP_Compras.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; } = null!;

    public DbSet<Store> Stores { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<DeliverySchedule> DeliverySchedules { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}