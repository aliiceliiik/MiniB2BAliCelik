using Microsoft.EntityFrameworkCore;
using MiniB2B.DataAccess.Context;
using MiniB2B.Entities.Dtos.Sliders;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public class SliderRepository : ISliderRepository
{
    private readonly MiniB2BDbContext _context;

    public SliderRepository(MiniB2BDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SliderDto>> GetActiveAsync()
    {
        return await _context.Sliders
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SliderDto
            {
                Id = s.Id,
                Title = s.Title,
                ImageUrl = s.ImageUrl,
                LinkUrl = s.LinkUrl
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<SliderFormDto>> GetAllAsync()
    {
        return await _context.Sliders
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SliderFormDto
            {
                Id = s.Id,
                Title = s.Title,
                ImageUrl = s.ImageUrl,
                LinkUrl = s.LinkUrl,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public Task<SliderFormDto?> GetFormAsync(int id)
    {
        return _context.Sliders
            .Where(s => s.Id == id)
            .Select(s => new SliderFormDto
            {
                Id = s.Id,
                Title = s.Title,
                ImageUrl = s.ImageUrl,
                LinkUrl = s.LinkUrl,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Slider?> GetByIdAsync(int id) => await _context.Sliders.FindAsync(id);

    public async Task AddAsync(Slider slider)
    {
        _context.Sliders.Add(slider);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}