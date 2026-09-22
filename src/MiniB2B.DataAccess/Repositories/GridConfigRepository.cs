using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Dtos.Grid;

namespace MiniB2B.DataAccess.Repositories;

public class GridConfigRepository : IGridConfigRepository
{
    private readonly MiniB2BDbContext _context;

    public GridConfigRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GridColumnDto>> GetVisibleColumnsAsync(string gridKey)
    {
        return await _context.GridColumnConfigs
            .Where(c => c.GridKey == gridKey && c.IsVisible)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new GridColumnDto
            {
                FieldName = c.FieldName,
                HeaderText = c.HeaderText,
                RenderType = c.RenderType,
                Width = c.Width,
                Alignment = c.Alignment,
                ShowOnDesktop = c.ShowOnDesktop,
                ShowOnTablet = c.ShowOnTablet,
                ShowOnMobile = c.ShowOnMobile
            })
            .ToListAsync();
    }
}