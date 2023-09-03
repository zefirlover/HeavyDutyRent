namespace Domain.DTOs;

public class OrderDto
{
    public int BuyerId { get; set; }
    public ICollection<GetMachineryDto> Machineries { get; set; }
}