using System.ComponentModel.DataAnnotations;

namespace Domain.Model;

public class Image
{
    [Key]
    public string Url { get; set; }
    public int MachineryId { get; set; }
    public Machinery Machinery { get; set; }
}