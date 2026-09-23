using MiniB2B.DataAccess.Repositories;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Sliders;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business.Services;

public class SliderService : ISliderService
{
    private readonly ISliderRepository _sliderRepository;

    public SliderService(ISliderRepository sliderRepository)
    {
        _sliderRepository = sliderRepository;
    }

    public Task<IReadOnlyList<SliderDto>> GetActiveAsync() => _sliderRepository.GetActiveAsync();

    public Task<IReadOnlyList<SliderFormDto>> GetAllAsync() => _sliderRepository.GetAllAsync();

    public Task<SliderFormDto?> GetForEditAsync(int id) => _sliderRepository.GetFormAsync(id);

    public async Task<ServiceResult> CreateAsync(SliderFormDto form)
    {
        if (string.IsNullOrWhiteSpace(form.ImageUrl))
            return ServiceResult.Failure("Slider görseli zorunludur.");

        var slider = new Slider
        {
            Title = form.Title.Trim(),
            ImageUrl = form.ImageUrl,
            LinkUrl = NormalizeLink(form.LinkUrl),
            DisplayOrder = form.DisplayOrder,
            IsActive = form.IsActive
        };

        await _sliderRepository.AddAsync(slider);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateAsync(SliderFormDto form)
    {
        var slider = await _sliderRepository.GetByIdAsync(form.Id);
        if (slider is null)
            return ServiceResult.Failure("Slider bulunamadı.");

        slider.Title = form.Title.Trim();
        slider.LinkUrl = NormalizeLink(form.LinkUrl);
        slider.DisplayOrder = form.DisplayOrder;
        slider.IsActive = form.IsActive;

        if (form.ImageUrl is not null)
            slider.ImageUrl = form.ImageUrl;

        await _sliderRepository.SaveChangesAsync();
        return ServiceResult.Success();
    }

    private static string? NormalizeLink(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return null;

        link = link.Trim();

        return link.StartsWith('/') || link.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? link
            : null;
    }
}