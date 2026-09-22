using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Web.Auth;

namespace MiniB2B.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync(User.GetUserId());
        return View(cart);
    }
}