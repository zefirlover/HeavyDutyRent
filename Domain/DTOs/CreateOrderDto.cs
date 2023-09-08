namespace Domain.DTOs;

public class CreateOrderDto
{
    public int BuyerId { get; set; }
    public string Status { get; set; }
    public List<int> MachineriesIds { get; set; }
}