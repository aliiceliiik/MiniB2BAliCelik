using MiniB2B.Entities.Dtos.Sliders;
using MiniB2B.Entities.Models;

namespace MiniB2B.DataAccess.Repositories;

public interface ISliderRepository
{
    Task<IReadOnlyList<SliderDto>> GetActiveAsync();
    Task<IReadOnlyList<SliderFormDto>> GetAllAsync();
    Task<SliderFormDto?> GetFormAsync(int id);
    Task<Slider?> GetByIdAsync(int id);
    Task AddAsync(Slider slider);
    Task SaveChangesAsync();
}