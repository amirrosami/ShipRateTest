using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces.Marketing;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{

    [Authorize]
    public class MarketingDialerController : Controller
    {
        private readonly IMarketingCallService _callService;
        public MarketingDialerController(IMarketingCallService callService)
        {
            _callService = callService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var recentCalls = await _callService.GetRecentCallsAsync(10);
            return View(recentCalls);
        }

        [HttpPost]
        public async Task<IActionResult> InitiateCall(QuickCallInitiateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return BadRequest("شماره تلفن الزامی است.");

            // در عمل IP تلفن و داخلی از پروفایل کاربر فعلی (User Profile) خوانده می‌شود:
            string currentUserId = User.Identity?.Name ?? "User1";
            string userExtension = "105";
            string userPhoneIp = "192.168.1.50"; // IP تلفن رومیزی سیسکو بازاریاب

            var result = await _callService.StartCallAsync(dto, currentUserId, userExtension, userPhoneIp);

            // بازگرداندن پنل وضعیت تماس و ثانیه‌شمار با HTMX
            ViewBag.Success = result.Success;
            ViewBag.Message = result.Message;
            ViewBag.CallLogId = result.CallLogId;
            ViewBag.PhoneNumber = dto.PhoneNumber;
            ViewBag.CustomerName = dto.CustomerName;

            return PartialView("_ActiveCallPanel");
        }

        [HttpPost]
        public async Task<IActionResult> SaveResult(SaveCallResultDto dto)
        {
            var success = await _callService.SaveCallResultAsync(dto);
            if (!success) return BadRequest("خطا در ثبت نتیجه.");

            var recentCalls = await _callService.GetRecentCallsAsync(10);
            return PartialView("_RecentCallsGrid", recentCalls);
        }
    }
}
