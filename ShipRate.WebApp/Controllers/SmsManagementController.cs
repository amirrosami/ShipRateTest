using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.Messaging;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class SmsManagementController : Controller
    {
        private readonly ISmsManagementService _smsService;

        public SmsManagementController(ISmsManagementService smsService)
        {
            _smsService = smsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new SmsDashboardViewModel
            {
                Stats = await _smsService.GetSmsStatsAsync()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> OutboxList([FromBody] SmsOutboxFilterDto filter)
        {
            var items = await _smsService.GetPagedOutboxAsync(filter);
            return PartialView("_OutboxListTable", items);
        }

        [HttpPost]
        public async Task<IActionResult> IncomingList([FromBody] SmsIncomingFilterDto filter)
        {
            var items = await _smsService.GetPagedIncomingAsync(filter);
            return PartialView("_IncomingListTable", items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendManualSms([FromBody] SendManualSmsDto dto)
        {
            var result = await _smsService.SendManualSmsAsync(dto);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> RetryFailedSms(Guid id)
        {
            var result = await _smsService.RetryFailedSmsAsync(id);
            return Json(new { success = result.Success, message = result.Message });
        }
    }
}
