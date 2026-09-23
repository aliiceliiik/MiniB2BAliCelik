using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniB2B.Business.Services;
using MiniB2B.Web.Models;
using System.Diagnostics;
using MiniB2B.Business.Services;

namespace MiniB2B.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISliderService _sliderService;

    public HomeController(ILogger<HomeController> logger, ISliderService sliderService)
    {
        _logger = logger;
        _sliderService = sliderService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _sliderService.GetActiveAsync());
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult HttpError(int code)
    {
        ViewData["Code"] = code;

        ViewData["Message"] = code switch
        {
            404 => "Aradýðýnýz sayfa bulunamadý.",
            403 => "Bu sayfaya eriþim yetkiniz bulunmuyor.",
            400 => "Geçersiz bir istek gönderildi.",
            _ => "Ýsteðiniz iþlenirken bir sorun oluþtu."
        };

        return View("HttpError");
    }
}
