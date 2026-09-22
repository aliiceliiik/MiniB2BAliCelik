using Microsoft.AspNetCore.Mvc;

namespace MiniB2B.Web.Areas.Admin.Controllers;

public class DashboardController : AdminControllerBase
{
    public IActionResult Index()
    {
        return View();
    }
}