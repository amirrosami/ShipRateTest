using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class Dashboard3Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
