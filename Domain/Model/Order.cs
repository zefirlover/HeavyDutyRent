using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model;

public class Order
{
    public int Id { get; set; }
    public string Status { get; set; }
    public int BuyerId { get; set; }
    public Buyer Buyer { get; set; }
    public ICollection<Machinery> Machineries { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreatedAt { get; set; }
}