using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BillOfLadings;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using System.Text.Json;



namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class BLManagementController : Controller
    {
        private readonly IBLManagementService _blService;
        private readonly ITransportDocumentService _transportDocumentService;
        private readonly INOAService _noaService;
        private readonly IDeliveryOrderService _doService;
        private const string BLSearchSessionKey = "BLManagement_LastSearchCriteria";


        public BLManagementController(IBLManagementService blService, ITransportDocumentService transportDocumentService, INOAService noaService, IDeliveryOrderService doService)
        {
            _blService = blService;
            _transportDocumentService = transportDocumentService;
            _noaService = noaService;
            _doService = doService;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        // بارگذاری اولیه کل صفحه
        [HttpGet]
        [Authorize("Operations.BLManagement.View")]
        public async Task<IActionResult> Index(BLManagementSearchRequest request)
        {
            //var model = await _blService.GetManagementPageDataAsync(request);
            //return View(model);

            bool isRequestEmpty = string.IsNullOrWhiteSpace(request.BLNo) && request.Page <= 1;
            if (isRequestEmpty)
            {
                // تلاش برای بازیابی آخرین سرچ از Session
                var savedRequest = HttpContext.Session.GetObject<BLManagementSearchRequest>(BLSearchSessionKey);
                if (savedRequest != null)
                {
                    request = savedRequest;
                }
            }
            else
            {
                // در صورت ارسال پارامتر مستقیم در متد Get، ذخیره در سشن
                HttpContext.Session.SetObject(BLSearchSessionKey, request);
            }

            var model = await _blService.GetManagementPageDataAsync(request);
            // ارسال فیلترهای فعال به View جهت پر کردن مجدد اینپوت‌ها (Inputs / Dropdowns)
            ViewBag.CurrentFilter = request;
            return View(model);
        }

        // اکشن اختصاصی برای درخواست‌های HTMX (فقط بازگرداندن Partial View بدون Layout اصلی)
        [HttpPost]
        public async Task<IActionResult> SearchList(BLManagementSearchRequest request)
        {
            //var model = await _blService.GetManagementPageDataAsync(request);
            //return PartialView("_BLListPartial", model);

            HttpContext.Session.SetObject(BLSearchSessionKey, request);

            var model = await _blService.GetManagementPageDataAsync(request);
            return PartialView("_BLListPartial", model);




        }

        [HttpPost]
        public IActionResult ResetSearch()
        {
            HttpContext.Session.Remove(BLSearchSessionKey);
            return RedirectToAction(nameof(Index));
        }



        // اکشن مودال حذف بارنامه
        [HttpGet]
        public IActionResult ConfirmDelete(Guid id)
        {
            ViewBag.BLId = id;
            return PartialView("_DeleteModalPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeletePost(Guid id)
        {
            // شناسه کاربر فرضی
            var currentUserId = Guid.NewGuid();
            var success = await _blService.DeleteBLAsync(id, currentUserId);

            if (success)
            {
                // پس از حذف موفقیت‌آمیز، جدول را با HTMX رفرش می‌کنیم
                return Response.Headers.ContainsKey("HX-Trigger")
                    ? StatusCode(200)
                    : RedirectToAction(nameof(Index));
            }
            return BadRequest("Failed to delete the selected Bill of Lading.");
        }
        [HttpGet]
        public async Task<IActionResult> MasterConsole(Guid id, Guid jobId)
        {
            var model = await _blService.GetMasterConsoleAsync(id);
            if (model is null)
                return NotFound();
            return View(model);
        }
        public async Task<IActionResult> BLSummary(Guid blId, CancellationToken cancellationToken)
        {
            var vm = await _blService.GetBLSummaryAsync(blId, cancellationToken);
            if (vm == null)
                return Content("<div class='alert alert-danger mb-0'>BL not found.</div>", "text/html");
            var autoFillData = new
            {
                arrivalDate = vm.ArrivalDate?.ToString("yyyy-MM-dd"),
                // هر فیلد دیگری که از BL باید کپی شود

            };
            Response.Headers.Add("HX-Trigger", Newtonsoft.Json.JsonConvert.SerializeObject(new { fillBLDetails = autoFillData }));
            return PartialView("_BLSummaryCard", vm);
        }
        public async Task<IActionResult> GetBlInvoices(Guid blId, Guid jobId, string filterType)
        {
            var vm = new BLInvoicesVm();
            var invoices = await _blService.GetInvoicesByBlIdAsync(blId, filterType);
            vm.Invoices = invoices;
            vm.BLId = blId;
            vm.JobId = jobId;
            vm.FilterType = filterType;
            return PartialView("_BlInvoicesGrid", vm);
        }
        public async Task<IActionResult> NewNOA(Guid blId)
        {
            var vm = await _noaService.GetNewFormForBLAsync(blId);
            return PartialView("~/Views/NOA/_NOAForm.cshtml", vm);
        }
        public async Task<IActionResult> GetDetentionsGrid(Guid id, CancellationToken cancellationToken, bool showInactive = false)
        {
            var vm = await _blService.GetDetentionsByBlAsync(id, cancellationToken);
            if (!showInactive)
                vm.Items = vm.Items?.Where(x => x.StatusId != -1).ToList();
            vm.ShowInActive = showInactive;
            if (vm == null)
            {
                return PartialView("_DetentionsGrid", new MasterConsoleDetentionsVm
                {
                    BLId = id,
                });
            }
            return PartialView("_DetentionsGrid", vm);
        }

        public async Task<IActionResult> GetNOAsGrid(Guid id, CancellationToken cancellationToken)
        {
            var vm = await _noaService.GetPageAsync(new Application.DTOs.NOAFilterDto { Page = 1, BLId = id }, cancellationToken);
            if (vm == null)
            {
                return PartialView("_BLNOAList", new DeliveryOrderDetentionsVm
                {
                    DeliveryOrderId = id
                });
            }
            return PartialView("_BLNOAList", vm);
        }
        public async Task<IActionResult> GetDOsGrid(Guid id, CancellationToken cancellationToken)
        {
            var vm = await _doService.GetPagedListAsync(new DeliveryOrderSearchFilter { Page = 1, BLId = id }, cancellationToken);
            if (vm == null)
            {
                return PartialView("_BLDoListGrid", new DeliveryOrderDetentionsVm
                {
                    DeliveryOrderId = id
                });
            }
            return PartialView("_BLDoListGrid", vm);
        }

        [HttpGet]
        public async Task<IActionResult> PreviewDoPrices(Guid blId, int clearanceTypeId = 1, decimal taxRate = 10.00m)
        {
            var items = await _blService.GetDoPricePreviewAsync(blId, clearanceTypeId, taxRate);

            var vm = new DoPricePreviewViewModel
            {
                BLId = blId,
                ClearanceTypeId = clearanceTypeId,
                TaxRate = taxRate,
                Items = items
            };

            // اگر درخواست از طریق HTMX و فقط برای جدول بود
            if (Request.Headers.ContainsKey("HX-Request") && Request.Headers["HX-Target"] == "do-price-grid-area")
            {
                return PartialView("_PreviewDoPricesGrid", vm);
            }
            return PartialView("_PreviewDoPricesModal", vm);
        }
        [HttpGet]
        public async Task<IActionResult> GetFinalVoyageSummary(Guid blId)
        {
            try
            {
                var vm = await _transportDocumentService.GetFinalVoyageForBLAsync(blId);
                return PartialView("_FinalVoyageSummary", vm);
            }
            catch (Exception ex)
            {
                // متن خطا را مستقیم ببینید
                return StatusCode(500, $"Error: {ex.Message} \nInner: {ex.InnerException?.Message} \n{ex.StackTrace}");
            }
        }
    }
}
