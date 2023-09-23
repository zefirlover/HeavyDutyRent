namespace Domain.DTOs;

public class GetCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<GetMachineryDto> Machineries { get; set; }
}