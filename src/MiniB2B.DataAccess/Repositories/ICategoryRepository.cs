using MiniB2B.Entities.Dtos.Categories;

namespace MiniB2B.DataAccess.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<CategoryOptionDto>> GetOptionsAsync();
    Task<bool> ExistsAsync(int id);
}