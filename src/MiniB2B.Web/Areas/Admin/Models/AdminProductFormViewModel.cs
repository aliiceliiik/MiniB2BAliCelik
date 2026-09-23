using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Web.Areas.Admin.Models;

public class AdminProductFormViewModel
{
    public ProductFormDto Product { get; set; } = new();

    public IFormFile? ImageFile { get; set; }

    [BindNever]
    public List<SelectListItem> Categories { get; set; } = [];

    [BindNever]
    public string? CurrentImageUrl { get; set; }

    public bool IsEdit => Product.Id > 0;
}