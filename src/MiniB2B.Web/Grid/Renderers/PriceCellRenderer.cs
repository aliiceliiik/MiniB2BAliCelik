using System.Globalization;
using Microsoft.AspNetCore.Html;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Grid.Renderers;

public class PriceCellRenderer : IGridCellRenderer
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");

    public GridRenderType RenderType => GridRenderType.Price;

    public IHtmlContent Render(object? value)
    {
        var text = value is decimal price ? price.ToString("C2", TurkishCulture) : "-";
        return new HtmlContentBuilder().Append(text);
    }
}