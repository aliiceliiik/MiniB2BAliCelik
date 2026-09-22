using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Web.Auth;

namespace MiniB2B.Web.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly ICartService _cartService;

    public CartSummaryViewComponent(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = 0;

        if (UserClaimsPrincipal.Identity?.IsAuthenticated == true)
            count = await _cartService.GetItemCountAsync(UserClaimsPrincipal.GetUserId());

        return View(count);
    }
}