using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Sliders;

namespace MiniB2B.Business.Services;

public interface ISliderService
{
    Task<IReadOnlyList<SliderDto>> GetActiveAsync();
    Task<IReadOnlyList<SliderFormDto>> GetAllAsync();
    Task<SliderFormDto?> GetForEditAsync(int id);
    Task<ServiceResult> CreateAsync(SliderFormDto form);
    Task<ServiceResult> UpdateAsync(SliderFormDto form);
}