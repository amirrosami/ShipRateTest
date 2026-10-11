using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class HubController : Controller
    {
        public IActionResult BaseInfo() => View();
        public IActionResult BaseOperations() => View();
        public IActionResult Operations() => View();
        public IActionResult Import() => View();
        public IActionResult Containers() => View();
        public IActionResult Transit() => View();
        public IActionResult Finance() => View();
        public IActionResult Crm() => View();
        public IActionResult Marketing() => View();
        public IActionResult PettyCash() => View();
        public IActionResult Inventory() => View();
        public IActionResult Utilities() => View();

        [Authorize(Roles = "External Party")]
        public IActionResult ExternalPortal() => View();

        [Authorize(Roles = "Admin")]
        public IActionResult Administration() => View();
    }
}
