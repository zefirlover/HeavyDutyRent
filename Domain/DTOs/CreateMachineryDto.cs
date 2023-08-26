namespace Domain.DTOs;

public class CreateMachineryDto
{
    public string Name { get; set; }
    public string AddressLine { get; set; }
    public string Price { get; set; }
    public int SellerId { get; set; }
}