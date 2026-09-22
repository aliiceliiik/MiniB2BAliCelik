using MiniB2B.Entities.Enums;
using MiniB2B.Web.Grid.Renderers;

namespace MiniB2B.Web.Grid;

public class GridCellRendererResolver
{
    private readonly Dictionary<GridRenderType, IGridCellRenderer> _renderers;
    private readonly IGridCellRenderer _fallback;

    public GridCellRendererResolver(IEnumerable<IGridCellRenderer> renderers)
    {
        _renderers = renderers.ToDictionary(r => r.RenderType);
        _fallback = _renderers[GridRenderType.Text];
    }

    public IGridCellRenderer Get(GridRenderType renderType)
    {
        return _renderers.TryGetValue(renderType, out var renderer) ? renderer : _fallback;
    }
}