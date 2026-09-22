using MiniB2B.Entities.Dtos.Grid;

namespace MiniB2B.DataAccess.Repositories;

public interface IGridConfigRepository
{
    Task<IReadOnlyList<GridColumnDto>> GetVisibleColumnsAsync(string gridKey);
}