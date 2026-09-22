using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Dtos.Grid;

public class GridColumnDto
{
    public string FieldName { get; init; } = null!;
    public string HeaderText { get; init; } = null!;
    public GridRenderType RenderType { get; init; }
    public string? Width { get; init; }
    public string Alignment { get; init; } = "left";
    public bool ShowOnDesktop { get; init; }
    public bool ShowOnTablet { get; init; }
    public bool ShowOnMobile { get; init; }
}