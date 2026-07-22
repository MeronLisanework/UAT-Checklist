using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECIMS.Web.Models;

namespace ECIMS.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
  public IActionResult Index()
{
    if (User.Identity?.IsAuthenticated == true && User.IsInRole(ECIMS.Web.Models.Enums.RoleNames.FunctionalConsultant))
    {
        return RedirectToAction("Index", "Consultant");
    }

    return View();
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}