using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Dtos.Orders;
using MiniB2B.Entities.Enums;

namespace MiniB2B.Web.Areas.Admin.Controllers;

public class OrdersController : AdminControllerBase
{
    private readonly IAdminOrderService _orderService;

    public OrdersController(IAdminOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index([FromQuery] AdminOrderSearchRequest filter)
    {
        ViewData["Filter"] = filter;
        return View(await _orderService.SearchAsync(filter));
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetDetailAsync(id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
    {
        var result = await _orderService.ChangeStatusAsync(id, status);

        if (result.IsSuccess)
            TempData["Success"] = status == OrderStatus.Approved
                ? "Sipariş onaylandı."
                : "Sipariş reddedildi ve ürünler stoğa iade edildi.";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Details), new { id });
    }
}