namespace MiniB2B.Entities.Models;

public class Slider : BaseEntity
{
    public string Title { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string? LinkUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}