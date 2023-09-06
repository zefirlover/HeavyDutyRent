using Domain;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class HeavyDutyRentDbContextSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Buyer>().HasData(
            new Buyer { Id = 1, Email = "zefirlover@outlook.com", UserName = "zefirlover", Name = "zefir", Surname = "Lover", PhoneNumber = "+380963333333", PasswordHash = "HuskTheBest7355608", PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0 }
        );

        modelBuilder.Entity<Seller>().HasData(
            new Seller { Id = 1, AddressLine = "sample", UserName = "TheSeller", Email = "theseller@gmail.com", PasswordHash = "HuskTheBest75_", PhoneNumber = "+380968886969", PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0 }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "CategoryName" }
        );
/*
        var machinery = new Machinery
            { Id = 1, Name = "Tracktor", AddressLine = "SampleAddress", Price = "300$", SellerId = 1 };
        
        modelBuilder.Entity<Machinery>().HasData(machinery);
        
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, BuyerId = 1, Status = "test", Machineries = new List<Machinery> { machinery }}
        );*/
        /*
        modelBuilder.Entity("MachineryOrder").HasData(
            new
            {
                OrdersId = 1,    // The ID of the order
                MachineriesId = 1 // The ID of the machinery
            }
        );
        
        modelBuilder.Entity("MachineryOrder").HasData(new { OrdersId = 1, MachineriesId = 1 });*/
        
        var machinery = new Machinery
        {
            Id = 1,
            Name = "Tracktor",
            AddressLine = "SampleAddress",
            Price = "300$",
            SellerId = 1,
            //Orders = new List<Order>()
        };

        var timeNow = DateTimeOffset.UtcNow;
        var order = new Order
        {
            Id = 1,
            BuyerId = 1,
            Status = "test",
            CreatedAt = timeNow
        };

        //machinery.Orders.Add(order);
        
        modelBuilder.Entity<Machinery>().HasData(machinery);
        modelBuilder.Entity<Order>().HasData(order);
    }
}