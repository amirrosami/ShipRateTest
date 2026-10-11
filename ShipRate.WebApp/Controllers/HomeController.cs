using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Dashboard;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels.Dashboard;

namespace ShipRate.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IActivityLogService _activityLogService;
        private readonly IJobService _jobService;
        private readonly IDashboardService _dashboardService;

        public HomeController(IActivityLogService activityLogService, IJobService jobService, IDashboardService dashboardService)
        {
            _activityLogService = activityLogService;
            _jobService = jobService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";
            ViewData["PageIcon"] = "fas fa-tachometer-alt";

            var activeShipmentsCount = await _jobService.GetActiveShipmentsAsync();
            ViewBag.ActiveShipments = activeShipmentsCount.ToString("N0");



            var companyId = 1;

            var shipmentVolume = await _jobService
                .GetShipmentVolumeChartAsync(
                    companyId,
                    "weekly",
                    default);

            var transportationModes = await _jobService
                .GetTransportationModeChartAsync(
                    companyId,
                    default);

            var monthlyFinanceByCurrencies = await _dashboardService
                .GetMonthlyFinanceByCurrencyAsync(
                    companyId,
                    default);

            var topShippingRoutes = await _jobService
                   .GetTopShippingRoutesAsync(
                       companyId,
                       take: 5,
                       default);


            var customerGrowth = await _dashboardService.GetCustomerGrowthChartAsync(default);
            var model = new DashboardVm
            {
                ShipmentVolume = shipmentVolume,
                TransportationModes = transportationModes,
                MonthlyFinanceByCurrencies = monthlyFinanceByCurrencies,
                TopShippingRoutes = topShippingRoutes,
                CustomerGrowth = customerGrowth
            };

            return View(model);


        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            ViewData["PageIcon"] = "fas fa-shield-alt";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ActivityFeedWidget(int take = 8)
        {
            var items = await _activityLogService.GetRecentActivitiesAsync(take);
            return PartialView("_ActivityFeedWidget", items);
        }


        [HttpGet]
        public async Task<IActionResult> ShipmentVolumeData(
            string range = "weekly",
            CancellationToken cancellationToken = default)
        {
            var allowedRanges = new[] { "weekly", "monthly", "yearly" };

            if (!allowedRanges.Contains(range, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Invalid range."
                });
            }

            var companyId = 1;

            var result = await _jobService.GetShipmentVolumeChartAsync(
                companyId,
                range,
                cancellationToken);

            return Json(result);
        }










    }
}
