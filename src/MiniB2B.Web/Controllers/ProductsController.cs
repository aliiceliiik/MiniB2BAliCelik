using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Constants;
using MiniB2B.Entities.Dtos.Products;
using MiniB2B.Web.Models;

namespace MiniB2B.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IGridConfigService _gridConfigService;

    public ProductsController(IProductService productService, IGridConfigService gridConfigService)
    {
        _productService = productService;
        _gridConfigService = gridConfigService;
    }

    public async Task<IActionResult> Index([FromQuery] ProductSearchRequest request)
    {
        var columns = await _gridConfigService.GetColumnsAsync(GridKeys.ProductList);
        var products = await _productService.SearchAsync(request);

        var model = new ProductListViewModel
        {
            Columns = columns,
            Products = products,
            SearchTerm = request.SearchTerm
        };

        return View(model);
    }
}