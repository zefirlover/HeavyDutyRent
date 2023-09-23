namespace Domain.DTOs;

public class GetMachineryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AddressLine { get; set; }
    public string Price { get; set; }
    public int SellerId { get; set; }
    public ICollection<ImageDto> Images { get; set; }
}