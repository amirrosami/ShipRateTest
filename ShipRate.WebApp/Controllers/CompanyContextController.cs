using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CompanyContextController : Controller
    {
        private readonly ICurrentCompanyService _currentCompanyService;

        public CompanyContextController(ICurrentCompanyService currentCompanyService)
        {
            _currentCompanyService = currentCompanyService;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(int companyId)
        {
            await _currentCompanyService.SetCurrentCompanyAsync(companyId);

            var returnUrl = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
