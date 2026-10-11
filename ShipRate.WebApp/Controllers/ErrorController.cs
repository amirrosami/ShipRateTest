using Microsoft.AspNetCore.Mvc;

namespace ShipRate.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(string errorMessage)
        {
            return View("index",errorMessage);
        }
    }
}
