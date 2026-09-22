using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Dtos.Products;

namespace MiniB2B.Web.Controllers.Api;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsApiController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request)
    {
        var result = await _productService.SearchAsync(request);
        return Ok(result);
    }
}