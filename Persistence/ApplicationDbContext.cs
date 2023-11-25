using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Buyer> Buyers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Machinery> Machineries { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Machinery>()
            .HasMany(e => e.Categories)
            .WithMany(e => e.Machineries);
        
        modelBuilder.Entity<Machinery>()
            .HasMany(e => e.Orders)
            .WithMany(e => e.Machineries);
        
        modelBuilder.Entity<Machinery>()
            .HasMany(e => e.ImagesUrls)
            .WithOne(e => e.Machinery)
            .HasForeignKey(e => e.MachineryId);

        modelBuilder.Entity<Seller>()
            .HasMany(e => e.Machineries)
            .WithOne(e => e.Seller)
            .HasForeignKey(e => e.SellerId);
        
        modelBuilder.Entity<Buyer>()
            .HasMany(e => e.Orders)
            .WithOne(e => e.Buyer)
            .HasForeignKey(e => e.BuyerId);
        
        base.OnModelCreating(modelBuilder);
        
        HeavyDutyRentDbContextSeedData.Seed(modelBuilder);
    }
}