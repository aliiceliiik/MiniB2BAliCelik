using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Web.Auth;

namespace MiniB2B.Web.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var orders = await _orderService.GetUserOrdersAsync(User.GetUserId(), page);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id, bool created = false)
    {
        var order = await _orderService.GetUserOrderDetailAsync(User.GetUserId(), id);

        if (order is null)
            return NotFound();

        ViewData["JustCreated"] = created;
        return View(order);
    }
}