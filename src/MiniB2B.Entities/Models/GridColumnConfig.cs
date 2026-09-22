using MiniB2B.Entities.Enums;

namespace MiniB2B.Entities.Models;

public class GridColumnConfig : BaseEntity
{
    public string GridKey { get; set; } = null!;
    public string FieldName { get; set; } = null!;
    public string HeaderText { get; set; } = null!;
    public GridRenderType RenderType { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? Width { get; set; }
    public string Alignment { get; set; } = "left";
    public bool ShowOnDesktop { get; set; } = true;
    public bool ShowOnTablet { get; set; } = true;
    public bool ShowOnMobile { get; set; } = true;
}