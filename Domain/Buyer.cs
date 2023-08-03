using Microsoft.AspNetCore.Identity;

namespace Domain;

public class Buyer : IdentityUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Surname { get; set; }
    public string? AddressLine { get; set; }
    
    public ICollection<Order>? Orders { get; set; }
}