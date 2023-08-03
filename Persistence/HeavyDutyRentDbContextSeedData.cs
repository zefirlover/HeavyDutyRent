using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class HeavyDutyRentDbContextSeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Buyer>().HasData(
            new Buyer { Id = 1, Email = "zefirlover@outlook.com", UserName = "zefirlover", Name = "zefir", Surname = "Lover", PhoneNumber = "+380963333333", PasswordHash = "HuskTheBest7355608", PhoneNumberConfirmed = false, TwoFactorEnabled = false, LockoutEnabled = false, AccessFailedCount = 0 }
        );
    }
}