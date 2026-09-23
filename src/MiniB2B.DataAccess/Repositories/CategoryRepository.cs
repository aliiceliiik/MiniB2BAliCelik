using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Dtos.Categories;

namespace MiniB2B.DataAccess.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MiniB2BDbContext _context;

    public CategoryRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryOptionDto>> GetOptionsAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionDto { Id = c.Id, Name = c.Name })
            .ToListAsync();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _context.Categories.AnyAsync(c => c.Id == id);
    }
}