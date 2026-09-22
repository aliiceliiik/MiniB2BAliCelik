using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Grid.Renderers;

public class StockStatusCellRenderer : IGridCellRenderer
{
    public GridRenderType RenderType => GridRenderType.StockStatus;

    public IHtmlContent Render(object? value)
    {
        var (text, cssClass) = value switch
        {
            StockStatus.Available => ("Var", "bg-success"),
            StockStatus.Critical => ("Kritik", "bg-warning text-dark"),
            StockStatus.OutOfStock => ("Yok", "bg-danger"),
            _ => ("-", "bg-secondary")
        };

        var badge = new TagBuilder("span");
        badge.AddCssClass($"badge {cssClass}");
        badge.InnerHtml.Append(text);
        return badge;
    }
}