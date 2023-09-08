namespace Domain.DTOs;

public class GetOrderDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public int BuyerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<GetMachineryDto> Machineries { get; set; }
}