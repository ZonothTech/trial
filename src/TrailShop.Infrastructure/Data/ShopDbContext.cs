using Microsoft.EntityFrameworkCore;
using TrailShop.Domain.Entities;

namespace TrailShop.Infrastructure.Data;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Username)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Lines)
            .WithOne()
            .HasForeignKey(l => l.OrderId);

        modelBuilder.Entity<OrderLine>()
            .HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId);
    }
}
