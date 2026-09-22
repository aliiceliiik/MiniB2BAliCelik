using Microsoft.AspNetCore.Html;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Grid.Renderers;

public class QuantityAddToCartCellRenderer : IGridCellRenderer
{
    public GridRenderType RenderType => GridRenderType.QuantityAddToCart;

    public IHtmlContent Render(object? value)
    {
        if (value is not int productId)
            return HtmlString.Empty;

        var html = $"""
            <div class="input-group input-group-sm">
                <input type="number" class="form-control js-qty" min="1" value="1" />
                <button type="button" class="btn btn-primary js-add-to-cart"
                        data-product-id="{productId}">Sepete Ekle</button>
            </div>
            """;

        return new HtmlString(html);
    }
}