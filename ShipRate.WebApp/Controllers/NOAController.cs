using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using System.Data;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    [Route("noa")]
    public sealed class NOAController : Controller
    {
        private readonly INOAService _service;
        private readonly IWebHostEnvironment _env;
        private readonly ICurrentUserService _currentUserService;


        public NOAController(INOAService service, IWebHostEnvironment env, ICurrentUserService currentUserService)
        {
            _service = service;
            _env = env;
            _currentUserService = currentUserService;
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }


        [HttpGet("")]
        [Authorize("ImportSpecific.NOAs.View")]
        public IActionResult Index() => View();

        [HttpGet("grid")]
        [Authorize("ImportSpecific.NOAs.View")]
        public async Task<IActionResult> Grid([FromQuery] NOAFilterDto filter, CancellationToken ct)
            => PartialView("_NOAList", await _service.GetPageAsync(filter ?? new NOAFilterDto(), ct));

        [HttpGet("search-bl")]
        [Authorize("ImportSpecific.NOAs.Edit")]
        public async Task<IActionResult> SearchBL([FromQuery] string searchTerm, CancellationToken ct)
        {
            searchTerm = searchTerm?.Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return PartialView("_SearchMessage", "BL number is required.");

            var result = await _service.SearchBLAsync(searchTerm, ct);

            if (!result.Found)
                return PartialView("_SearchMessage", result.Message ?? "BL not found.");

            if (result.Multiple)
                return PartialView("_SearchMessage", result.Message ?? "More than one BL matched.");

            if (result.Form is null)
                return PartialView("_SearchMessage", "BL found, but form data is missing.");

            var vm = result.Form.Id is null
                ? await _service.GetNewFormForBLAsync(result.Form.BLId, ct)
                : await _service.GetFormAsync(result.Form.Id.Value, ct);

            return PartialView("_NOAForm", vm);
        }

        [HttpGet("create")]
        [Authorize("ImportSpecific.NOAs.View")]
        public IActionResult Create()
        {
            // بازگرداندن یک پارشال ساده که شامل باکس جستجوی BL در داخل مودال است
            return PartialView("_SearchBLModalInit");
        }



        [HttpGet("form/{id:guid}")]
        [Authorize("ImportSpecific.NOAs.Edit")]
        public async Task<IActionResult> Form(Guid id, CancellationToken ct)
        {
            var vm = await _service.GetFormAsync(id, ct);
            return vm is null ? NotFound() : PartialView("_NOAForm", vm);
        }

        [HttpPost("save")]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.NOAs.Edit")]
        public async Task<IActionResult> Save([Bind(Prefix = "Form")][FromForm] NOAFormDto dto, CancellationToken ct)
        {
            // ۱. بررسی اعتبارسنجی مدل
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                    .Where(msg => !string.IsNullOrWhiteSpace(msg))
                    .ToList();

                var combinedError = string.Join("<br/>", errors);

                return PartialView("_SaveMessage", new SaveMessageVM
                {
                    Error = string.IsNullOrEmpty(combinedError) ? "Please fill all required fields correctly." : combinedError
                });
            }



            var result = await _service.SaveAsync(dto, ct);

            if (!result.Success)
                return PartialView("_SaveMessage", new SaveMessageVM { Error = result.Error });

            HxTrigger(new
            {
                refreshNOAGrid = true,
                toastSuccess = "Saved Sussefully"
            });



            return PartialView("_PriceGrid", new PriceGridVM
            {
                NOAId = result.Id!.Value,
                NOANumber = result.NOANumber,
                Prices = result.Prices ?? [],
                SpError = result.SpError,
                ChargeTypes = await _service.GetChargeTypesAsync(),
                ClearanceTypes = await _service.GetClearanceTypesAsync()
            });
        }
        [HttpPost("delete/{id:guid}")]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.NOAs.Edit")]
        public async Task<IActionResult> Delete(Guid id, [FromForm] NOAFilterDto filter, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            HxTrigger(new
            {
                refreshNOAGrid = true,
                toastSuccess = "Notice Of Arrival Deleted Sussefully"
            });
            return Ok();
            //return PartialView("_NOAList", await _service.GetPageAsync(filter ?? new NOAFilterDto(), ct));
        }
        [HttpGet("prices/{id:guid}")]
        [Authorize("ImportSpecific.NOAs.View")]
        public async Task<IActionResult> Prices(Guid id, CancellationToken ct)
            => PartialView("_PriceGrid", new PriceGridVM
            {
                NOAId = id,
                Prices = await _service.GetPricesAsync(id, ct),
                ChargeTypes = await _service.GetChargeTypesAsync(),
                ClearanceTypes = await _service.GetChargeTypesAsync()
            });
        [HttpGet("print/{id:guid}")]
        [Authorize("ImportSpecific.NOAs.Send")]
        public async Task<IActionResult> Print(Guid id, CancellationToken ct)
        {
            var (pdfBytes, fileName) = await _service.GenerateNoaPdfBytesAsync(id, ct);
            if (pdfBytes == null)
            {
                HxTrigger(new { toastFail = "Notice of Arrival not printable or not found." });
                return NoContent();
            }
            return File(pdfBytes, "application/pdf");
        }

        [HttpPost("save-prices")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePrices(PriceGridVM model, CancellationToken cancellationToken = default)
        {
            if (model == null || model.NOAId == Guid.Empty)
            {
                return BadRequest("Invalid NOA identifier.");
            }

            try
            {
                // 1. محاسبه مقدار Total برای تک تک ردیف‌ها
                if (model.Prices != null && model.Prices.Any())
                {
                    foreach (var item in model.Prices)
                    {
                        item.TotalValue = item.InitialValue + item.VATAmount;
                    }
                }

                // 2. صدا زدن متد بیزینس در سرویس NOA
                var result = await _service.SavePricesAsync(model, cancellationToken);


                // ۲. بررسی و ثبت ردیف جدید (Add)
                if (model.NewRow != null &&
                    model.NewRow.ChargeTypeId > 0 &&
                    model.NewRow.InitialValue > 0)
                {
                    // انتساب NOAId به ردیف جدید
                    model.NewRow.NOAId = model.NOAId;

                    // صدا زدن سرویس درج ردیف جدید
                    await _service.AddPriceAsync(model.NOAId, model.NewRow, cancellationToken);
                }


                // اگر سرویس خودش مدل رفرش‌شده برمی‌گرداند از آن استفاده کنید، وگرنه از دیتابیس مجدد واکشی کنید:
                //var updatedGrid = result ?? await _service.GetPricesByNOAIdAsync(model.NOAId, cancellationToken);
                var updatedGrid = await _service.GetPricesByNOAIdAsync(model.NOAId, cancellationToken);
                updatedGrid.ChargeTypes = await _service.GetChargeTypesAsync();
                updatedGrid.ClearanceTypes = await _service.GetClearanceTypesAsync();

                HxTrigger(new { toastSuccess = "Charges Updated Successfuly" });
                return PartialView("_PriceGrid", updatedGrid);
            }
            catch (Exception ex)
            {
                // ثبت در لاگ و ست کردن ارور
                model.SpError = ex.Message;
                Response.Headers.Append("HX-Trigger", "{\"notify\": {\"type\": \"error\", \"message\": \"Failed to save charges.\"}}");
                return PartialView("_PriceGrid", model);
            }
        }
        [HttpPost("CreateOutFromIn")]
        public async Task<IActionResult> CreateOutFromIn(Guid id, CancellationToken ct)
        {
            var newNoaId = await _service.CreateOutFromInAsync(id, ct);
            HxTrigger(new
            {
                refreshNOAGrid = true,
                toastSuccess = "Notice Of Arrival For Customer Created Sussefully"
            });
            return NoContent();
            //// دریافت داده‌های ویومدل NOA جدید جهت باز کردن مستقیم آن در فرم
            //var formViewModel = await _service.GetFormViewModelAsync(newNoaId, ct);

            //return PartialView("_NOAForm", formViewModel);
        }

        [HttpGet("send-history/{id:guid}")]
        [Authorize("ImportSpecific.NOAs.View")]
        public async Task<IActionResult> SendHistory(Guid id)
        {
            var vm = await _service.GetSendHubViewModelAsync(id);
            return PartialView("_NOASendHubModal", vm);
        }

        // ۲. پردازش ارسال پیامک/ایمیل/پورتال و برگشت مستقیم به مدال (POST)
        [ValidateAntiForgeryToken]
        [HttpPost("send-dispatch")]
        [Authorize("ImportSpecific.NOAs.Send")]
        public async Task<IActionResult> SendDispatch(NOASendRequestDto dto)
        {
            await _service.DispatchNOANotificationAsync(dto, _currentUserService.UserId.ToString());
            var vm = await _service.GetSendHubViewModelAsync(dto.NOAId);
            return PartialView("_NOASendHubModal", vm);
        }


        [HttpGet("prev")]
        public async Task<IActionResult> PreviewWebReport(int id)
        {
            var report = new WebReport();
            report.Report.Load("Reports/c.frx");
            var countries = new List<CountryDto>
            {
                new CountryDto { Id = 1,  Code = "IR" },
                new CountryDto { Id = 2,  Code = "DE" },
                new CountryDto { Id = 3, Code = "TR" }
            };
            report.Report.RegisterData(countries, "Countries");
            report.Report.GetDataSource("Countries").Enabled = true;
            report.Toolbar.Exports = new ExportMenuSettings()
            {
                ExportTypes = Exports.Excel2007
            };

            report.Toolbar.Exports = ExportMenuSettings.All;
            report.Toolbar.Exports.EnableSettings = true;
            report.Toolbar.Exports.Show = true;
            report.Report.Prepare();
            using MemoryStream ms = new MemoryStream();
            // تنظیمات تولبار (اختیاری)
            report.Toolbar.Exports.EnableSettings = true;
            return View("ReportPreview", report);
        }
    }
}
