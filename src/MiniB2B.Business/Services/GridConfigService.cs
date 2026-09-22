using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Grid;

namespace MiniB2B.Business.Services;

public class GridConfigService : IGridConfigService
{
    private readonly IGridConfigRepository _gridConfigRepository;

    public GridConfigService(IGridConfigRepository gridConfigRepository)
    {
        _gridConfigRepository = gridConfigRepository;
    }

    public Task<IReadOnlyList<GridColumnDto>> GetColumnsAsync(string gridKey)
    {
        return _gridConfigRepository.GetVisibleColumnsAsync(gridKey);
    }
}