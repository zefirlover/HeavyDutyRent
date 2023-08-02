namespace Domain;

public class Order
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public Buyer Buyer { get; set; }
    public ICollection<Machinery> Machineries { get; set; }
}