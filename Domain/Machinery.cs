namespace Domain;

public class Machinery
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AddressLine { get; set; }
    public string Price { get; set; }
    
    public int SellerId { get; set; }
    public Seller Seller { get; set; }
    
    public ICollection<Image> ImagesUrls { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Order> Orders { get; set; }
}