namespace Domain.DTOs;

public class BuyerDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? AddressLine { get; set; }
    
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    
    public string PasswordHash { get; set; }
    
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    
    public bool TwoFactorEnabled { get; set; }
    public bool LockoutEnabled { get; set; }
    
    public int AccessFailedCount { get; set; }
}