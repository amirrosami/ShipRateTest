using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class VoyageManagementController : Controller
    {
        private readonly IVoyageManagementService _voyageManagementService;

        public VoyageManagementController(IVoyageManagementService voyageManagementService)
        {
            _voyageManagementService = voyageManagementService;
        }

        [HttpGet]
        [Authorize("Operations.VoyagesManagement.View")]
        public async Task<IActionResult> Index()
        {
            var model = await _voyageManagementService.GetIndexModelAsync();
            return View(model);
        }

        [HttpGet]
        [Authorize("Operations.VoyagesManagement.View")]
        public async Task<IActionResult> Search([FromQuery] VoyageSearchRequestDto request)
        {
            var result = await _voyageManagementService.SearchAsync(request);
            ViewBag.SearchRequest = request;
            return PartialView("_VoyageGrid", result);
        }

        [HttpGet]
        [Authorize("Operations.VoyagesManagement.View")]
        public async Task<IActionResult> Timeline(Guid voyagePortId)
        {
            var model = await _voyageManagementService.GetTimelineAsync(voyagePortId);
            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_TimelineModal", model);
        }

        [HttpPost]
        [Authorize("Operations.VoyagesManagement.Handle")]
        public async Task<IActionResult> ExecuteAction(VoyageActionRequestDto request)
        {
            await _voyageManagementService.ExecuteActionAsync(request);
            return Ok();
        }
        [HttpGet]
        [Authorize("Operations.VoyagesManagement.Handle")]
        public async Task<IActionResult> ActionPanel(Guid voyagePortId)
        {
            var row = await _voyageManagementService.GetMonitoringRowAsync(voyagePortId);
            if (row == null)
            {
                return NotFound();
            }

            return PartialView("_ActionPanel", row);
        }

        [HttpGet]
        public async Task<IActionResult> GetPortsDropdown(string term)
        {
            // دریافت بنادر بر اساس جستجو (اگر term خالی بود مثلاً 10 مورد اول)
            var ports = await _voyageManagementService.SearchPortsAsync(term, take: 20);
            return PartialView("_PortDropdownItems", ports);
        }

        [HttpGet]
        public async Task<IActionResult> GetPortsJson(string? q, int take = 20)
        {
            // صدا زدن متد سرویس که قبلاً با NLookupDto پیاده کردیم
            var ports = await _voyageManagementService.SearchPortsAsync(q, take);

            // Select2 به صورت پیش‌فرض فیلدهای id و text می‌خواهد
            var results = ports.Select(p => new
            {
                id = p.Value,
                text = p.Text
            });
            return Json(results);
        }
    }
}
