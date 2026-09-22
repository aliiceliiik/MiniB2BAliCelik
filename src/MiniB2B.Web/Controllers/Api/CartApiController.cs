using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Dtos.Cart;
using MiniB2B.Entities.Dtos.Common;
using MiniB2B.Web.Auth;

namespace MiniB2B.Web.Controllers.Api;

[ApiController]
[Route("api/cart")]
public class CartApiController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartApiController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _cartService.GetCartAsync(User.GetUserId()));
    }

    [HttpPost("items")]
    public async Task<IActionResult> Add([FromBody] AddToCartRequest request)
    {
        return ToActionResult(await _cartService.AddItemAsync(User.GetUserId(), request));
    }

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> Update(int productId, [FromBody] UpdateCartItemRequest request)
    {
        return ToActionResult(await _cartService.UpdateItemAsync(User.GetUserId(), productId, request.Quantity));
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> Remove(int productId)
    {
        return ToActionResult(await _cartService.RemoveItemAsync(User.GetUserId(), productId));
    }

    private IActionResult ToActionResult(ServiceResult<CartDto> result)
    {
        return result.IsSuccess
            ? Ok(result.Data)
            : BadRequest(new { message = result.ErrorMessage });
    }
}