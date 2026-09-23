namespace MiniB2B.Entities.Dtos.Sliders;

public class SliderDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string ImageUrl { get; init; } = null!;
    public string? LinkUrl { get; init; }
}