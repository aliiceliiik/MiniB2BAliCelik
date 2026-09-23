using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Sliders;
using MiniB2B.Web.Areas.Admin.Models;
using MiniB2B.Web.Services;

namespace MiniB2B.Web.Areas.Admin.Controllers;

public class SlidersController : AdminControllerBase
{
    private const long MaxRequestSize = 3 * 1024 * 1024;

    private readonly ISliderService _sliderService;
    private readonly IImageStorage _imageStorage;

    public SlidersController(ISliderService sliderService, IImageStorage imageStorage)
    {
        _sliderService = sliderService;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index() => View(await _sliderService.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View("Form", new SliderFormViewModel());

    [HttpPost]
    [RequestSizeLimit(MaxRequestSize)]
    public Task<IActionResult> Create(SliderFormViewModel model)
    {
        model.Slider.Id = 0;
        return SaveAsync(model, form => _sliderService.CreateAsync(form), "Slider eklendi.");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var slider = await _sliderService.GetForEditAsync(id);
        if (slider is null)
            return NotFound();

        return View("Form", new SliderFormViewModel
        {
            Slider = slider,
            CurrentImageUrl = slider.ImageUrl
        });
    }

    [HttpPost]
    [RequestSizeLimit(MaxRequestSize)]
    public Task<IActionResult> Edit(int id, SliderFormViewModel model)
    {
        model.Slider.Id = id;
        return SaveAsync(model, form => _sliderService.UpdateAsync(form), "Slider güncellendi.");
    }

    private async Task<IActionResult> SaveAsync(
        SliderFormViewModel model,
        Func<SliderFormDto, Task<ServiceResult>> save,
        string successMessage)
    {
        model.Slider.ImageUrl = null;

        if (ModelState.IsValid && model.ImageFile is not null)
        {
            var upload = await _imageStorage.SaveAsync(model.ImageFile, "sliders");

            if (upload.IsSuccess)
                model.Slider.ImageUrl = upload.Data;
            else
                ModelState.AddModelError(nameof(model.ImageFile), upload.ErrorMessage!);
        }

        if (ModelState.IsValid)
        {
            var result = await save(model.Slider);

            if (result.IsSuccess)
            {
                TempData["Success"] = successMessage;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
        }

        if (model.IsEdit)
            model.CurrentImageUrl = (await _sliderService.GetForEditAsync(model.Slider.Id))?.ImageUrl;

        return View("Form", model);
    }
}