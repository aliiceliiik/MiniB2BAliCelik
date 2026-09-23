using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Web.Auth;

namespace MiniB2B.Web.Controllers.Api;

[ApiController]
[Route("api/orders")]
public class OrdersApiController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersApiController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var result = await _orderService.CreateOrderAsync(User.GetUserId());

        return result.IsSuccess
            ? Ok(result.Data)
            : BadRequest(new { message = result.ErrorMessage });
    }
}