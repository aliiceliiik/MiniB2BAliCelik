using Microsoft.AspNetCore.Html;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Grid.Renderers;

public interface IGridCellRenderer
{
    GridRenderType RenderType { get; }
    IHtmlContent Render(object? value);
}