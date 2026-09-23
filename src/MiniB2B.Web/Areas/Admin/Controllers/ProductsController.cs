using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Web.Areas.Admin.Models;
using MiniB2B.Web.Services;

namespace MiniB2B.Web.Areas.Admin.Controllers;

public class ProductsController : AdminControllerBase
{
    private const long MaxRequestSize = 3 * 1024 * 1024;

    private readonly IAdminProductService _productService;
    private readonly IImageStorage _imageStorage;

    public ProductsController(IAdminProductService productService, IImageStorage imageStorage)
    {
        _productService = productService;
        _imageStorage = imageStorage;
    }

    public async Task<IActionResult> Index([FromQuery] AdminProductSearchRequest filter)
    {
        var model = new AdminProductListViewModel
        {
            Products = await _productService.SearchAsync(filter),
            Categories = await _productService.GetCategoriesAsync(),
            Filter = filter
        };

        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new AdminProductFormViewModel();
        await PopulateAsync(model);
        return View("Form", model);
    }

    [HttpPost]
    [RequestSizeLimit(MaxRequestSize)]
    public Task<IActionResult> Create(AdminProductFormViewModel model)
    {
        model.Product.Id = 0;
        return SaveAsync(model, async form => await _productService.CreateAsync(form), "Ürün başarıyla eklendi.");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetForEditAsync(id);
        if (product is null)
            return NotFound();

        var model = new AdminProductFormViewModel { Product = product };
        await PopulateAsync(model);
        return View("Form", model);
    }

    [HttpPost]
    [RequestSizeLimit(MaxRequestSize)]
    public Task<IActionResult> Edit(int id, AdminProductFormViewModel model)
    {
        model.Product.Id = id;
        return SaveAsync(model, form => _productService.UpdateAsync(form), "Ürün başarıyla güncellendi.");
    }

    private async Task<IActionResult> SaveAsync(
    AdminProductFormViewModel model,
    Func<ProductFormDto, Task<ServiceResult>> save,
    string successMessage)
    {
        model.Product.ImageUrl = null;

        if (ModelState.IsValid && model.ImageFile is not null)
        {
            var upload = await _imageStorage.SaveProductImageAsync(model.ImageFile);

            if (upload.IsSuccess)
                model.Product.ImageUrl = upload.Data;
            else
                ModelState.AddModelError(nameof(model.ImageFile), upload.ErrorMessage!);
        }

        if (ModelState.IsValid)
        {
            var result = await save(model.Product);

            if (result.IsSuccess)
            {
                TempData["Success"] = successMessage;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
        }

        await PopulateAsync(model);
        return View("Form", model);
    }

    private async Task PopulateAsync(AdminProductFormViewModel model)
    {
        var categories = await _productService.GetCategoriesAsync();

        model.Categories = categories
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();

        if (model.IsEdit)
            model.CurrentImageUrl = (await _productService.GetForEditAsync(model.Product.Id))?.ImageUrl;
    }
}