using Microsoft.AspNetCore.Mvc.ModelBinding;
using MiniB2B.Entities.Dtos.Sliders;

namespace MiniB2B.Web.Areas.Admin.Models;

public class SliderFormViewModel
{
    public SliderFormDto Slider { get; set; } = new();

    public IFormFile? ImageFile { get; set; }

    [BindNever]
    public string? CurrentImageUrl { get; set; }

    public bool IsEdit => Slider.Id > 0;
}