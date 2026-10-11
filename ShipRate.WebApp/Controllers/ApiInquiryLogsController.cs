using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs.ApiIr;
using ShipRate.Application.Interfaces.Logs;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.WebApp.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public sealed class ApiInquiryLogsController : Controller
    {
        private readonly IApiInquiryLogService _apiInquiryLogService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ApiInquiryLogsController> _logger;

        public ApiInquiryLogsController(
            IApiInquiryLogService apiInquiryLogService,
            UserManager<ApplicationUser> userManager,
            ILogger<ApiInquiryLogsController> logger)
        {
            _apiInquiryLogService = apiInquiryLogService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            ApiInquiryLogFilterDto filter,
            CancellationToken ct)
        {
            var result = await _apiInquiryLogService.GetPagedAsync(filter, ct);

            var users = _userManager.Users
                .OrderBy(x => x.UserName)
                .Select(x => new
                {
                    x.Id,
                    Name = x.UserName
                })
                .ToList();

            var userNames = users.ToDictionary(
                x => x.Id,
                x => x.Name ?? x.Id.ToString());

            foreach (var item in result.Items)
            {
                if (item.UserId.HasValue &&
                    userNames.TryGetValue(item.UserId.Value, out var userName))
                {
                    item.UserName = userName;
                }
                else
                {
                    item.UserName = "System / Unknown";
                }
            }

            var vm = new ApiInquiryLogIndexVm
            {
                Filter = filter,
                Result = result,

                Users =
                [
                    new SelectListItem("All Users", ""),
                .. users.Select(x => new SelectListItem(
                    x.Name ?? x.Id.ToString(),
                    x.Id.ToString(),
                    filter.UserId?.ToString() == x.Id.ToString()))
                ]
            };

            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var result = await _apiInquiryLogService.GetDetailsAsync(id, ct);

            if (result is null)
                return NotFound();

            if (result.UserId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(result.UserId.Value.ToString());

                result.UserName = user?.UserName ?? result.UserId.Value.ToString();
            }
            else
            {
                result.UserName = "System / Unknown";
            }

            return PartialView("_Details", result);
        }
    }
}
