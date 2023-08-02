using Microsoft.AspNetCore.Identity;

namespace Domain;

public class Seller : IdentityUser
{
    public int Id { get; set; }
    public string AddressLine { get; set; }
    public string Telephone { get; set; }
    public string? LogoUrl { get; set; }
    
    public ICollection<Machinery> Machineries { get; set; }
}