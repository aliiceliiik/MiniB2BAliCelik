using MiniB2B.Entities.Dtos.Grid;

namespace MiniB2B.Business.Services;

public interface IGridConfigService
{
    Task<IReadOnlyList<GridColumnDto>> GetColumnsAsync(string gridKey);
}