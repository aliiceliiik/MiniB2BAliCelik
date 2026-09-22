using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Grid.Renderers;

public class ImageCellRenderer : IGridCellRenderer
{
    public GridRenderType RenderType => GridRenderType.Image;

    public IHtmlContent Render(object? value)
    {
        if (value is not string url || string.IsNullOrWhiteSpace(url))
            return new HtmlContentBuilder().Append("-");

        var img = new TagBuilder("img");
        img.Attributes["src"] = url;
        img.Attributes["alt"] = "Ürün görseli";
        img.Attributes["loading"] = "lazy";
        img.AddCssClass("img-thumbnail grid-thumb");
        img.TagRenderMode = TagRenderMode.SelfClosing;
        return img;
    }
}