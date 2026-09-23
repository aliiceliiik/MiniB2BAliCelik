using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Entities.Dtos.Users;
using MiniB2B.Web.Auth;

namespace MiniB2B.Web.Areas.Admin.Controllers;

public class UsersController : AdminControllerBase
{
    private readonly IAdminUserService _userService;

    public UsersController(IAdminUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index([FromQuery] AdminUserSearchRequest filter)
    {
        ViewData["Filter"] = filter;
        return View(await _userService.SearchAsync(filter));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetForEditAsync(id);
        if (user is null)
            return NotFound();

        ViewData["IsSelf"] = id == User.GetUserId();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UserEditDto form)
    {
        form.Id = id;
        ViewData["IsSelf"] = id == User.GetUserId();

        if (!ModelState.IsValid)
            return await RedisplayAsync(form);

        var result = await _userService.UpdateAsync(form, User.GetUserId());

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return await RedisplayAsync(form);
        }

        TempData["Success"] = "Kullanıcı bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> RedisplayAsync(UserEditDto form)
    {
        var original = await _userService.GetForEditAsync(form.Id);
        if (original is null)
            return NotFound();

        form.CreatedAt = original.CreatedAt;
        form.OrderCount = original.OrderCount;
        form.NewPassword = null;
        return View(form);
    }
}