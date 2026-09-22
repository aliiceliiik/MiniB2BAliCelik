using MiniB2B.Web.Grid.Renderers;

namespace MiniB2B.Web.Grid;

public static class GridServiceRegistration
{
    public static IServiceCollection AddGridRendering(this IServiceCollection services)
    {
        services.AddSingleton<IGridCellRenderer, TextCellRenderer>();
        services.AddSingleton<IGridCellRenderer, ImageCellRenderer>();
        services.AddSingleton<IGridCellRenderer, PriceCellRenderer>();
        services.AddSingleton<IGridCellRenderer, StockStatusCellRenderer>();
        services.AddSingleton<IGridCellRenderer, QuantityAddToCartCellRenderer>();
        services.AddSingleton<GridCellRendererResolver>();

        return services;
    }
}