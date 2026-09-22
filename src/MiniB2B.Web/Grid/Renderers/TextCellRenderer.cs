using Microsoft.AspNetCore.Html;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Grid.Renderers;

public class TextCellRenderer : IGridCellRenderer
{
    public GridRenderType RenderType => GridRenderType.Text;

    public IHtmlContent Render(object? value)
    {
        return new HtmlContentBuilder().Append(value?.ToString() ?? "-");
    }
}