namespace Domain.Model;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Machinery> Machineries { get; set; }
}