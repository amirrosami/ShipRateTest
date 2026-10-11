using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        public IActionResult UnderConstruction(string? page = null)
        {
            ViewBag.PageName = page;
            return View();
        }
    }
}
