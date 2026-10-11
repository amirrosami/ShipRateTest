using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DetentionsController : Controller
    {
        private readonly IDetentionService _detentionService;
        private readonly IBaseDataService _baseDataService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ITransportDocumentService _transportDocumentService;
        private readonly IJobService _jobService;

        public DetentionsController(IDetentionService detentionService, IBaseDataService baseDataService, IExchangeRateService exchangeRateService, ITransportDocumentService transportDocumentService, IJobService jobService)
        {
            _detentionService = detentionService;
            _baseDataService = baseDataService;
            _exchangeRateService = exchangeRateService;
            _transportDocumentService = transportDocumentService;
            _jobService = jobService;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }


        public async Task<IActionResult> Index(DetentionSearchVm search, CancellationToken cancellationToken)
        {
            search.Normalize();
            var model = await _detentionService.GetPagedListAsync(search);
            if (Request.Headers["HX-Request"].Count > 0)
                return PartialView("_DetentionGrid", model);

            ViewBag.DetentionStatuses = await _detentionService.GetDetentionStatusesAsync(cancellationToken);


            return View(model);
        }

        public async Task<IActionResult> GetItems(Guid id)
        {
            var items = await _detentionService.GetItemsAsync(id);
            return PartialView("_DetentionItemsGrid", items);
        }

        public async Task<IActionResult> GetItemDetails(Guid id)
        {
            var details = await _detentionService.GetItemDetailsAsync(id);
            return PartialView("_DetentionItemDetailsGrid", details);
        }
        [HttpGet]
        public async Task<IActionResult> Create(Guid? blId, Guid? bookingId, CancellationToken cancellationToken)
        {
            await FillCreateLookupsAsync(blId, bookingId, cancellationToken);
            var model = new CreateDetentionVm
            {
                DetentionToDate = DateTime.Today,
                BLId = blId,
                BookingId = bookingId,
                MainCurrencyId = 1,
                FinalCurrencyId = 2,
                ExchangeRateValue = await _exchangeRateService.GetFormalPortsExchangeRateValueAsync(1, 2, DateTime.Now),
                CompanyId = 1,
                TradeTypeId = 1,
                FinanceDirectionId = 1,
                TariffBookId = blId == null || blId == Guid.Empty ? null : await _transportDocumentService.GetPODTariffBookIdAsync(blId.Value),
            };
            if (blId != null && blId.HasValue && blId != Guid.Empty)
            {
                var clientId = await _transportDocumentService.GetInvoiceClientIdAsync(blId.Value);
                if (clientId != null && clientId.HasValue && clientId != Guid.Empty) model.PartyId = clientId.Value;
            }
            return PartialView("_CreateDetentionModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calculate(
            CreateDetentionVm model,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (model.BLId.HasValue && model.BLId != Guid.Empty)
                {
                    var tariffValidate = await _detentionService.ValidateTariffRulesAsync(model.BLId.Value, model.TariffBookId.Value);
                    if (!tariffValidate)
                    {
                        var missingTypes = await _detentionService.GetMissingContainerTypesAsync(model.BLId.Value, model.TariffBookId.Value);
                        if (missingTypes.Any())
                        {
                            ModelState.AddModelError("", "No Tariff Defined For Container Types :" + string.Join(", ", missingTypes) + ".");
                        }
                    }
                }
                else if (model.BookingId.HasValue && model.BookingId != Guid.Empty)
                {
                    var tariffValidate = await _detentionService.ValidateTariffRulesForBookingAsync(model.BookingId.Value, model.TariffBookId.Value);
                    if (!tariffValidate)
                    {
                        var missingTypes = await _detentionService.GetMissingContainerTypesForBookingAsync(model.BookingId.Value, model.TariffBookId.Value);
                        if (missingTypes.Any())
                        {
                            ModelState.AddModelError("", "No Tariff Defined For Container Types :" + string.Join(", ", missingTypes) + ".");
                        }
                    }
                }
            }


            if (!ModelState.IsValid)
            {

                // اگر خطایی وجود دارد
                if (!ModelState.IsValid)
                {
                    // یک Alert تمیز مستقیماً داخل #detention-calculation-result تزریق می‌شود
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .Where(msg => !string.IsNullOrWhiteSpace(msg))
                        .ToList();

                    var errorHtml = $@"
                                    <div class='alert alert-dismissible fade show shadow-sm py-2 px-3 mb-0'
                                         role='alert'
                                         style='background-color:#f8d7da !important;
                                                background-image:none !important;
                                                border:1px solid #f1aeb5 !important;
                                                color:#58151c !important;'>

                                        <div class='d-flex align-items-center mb-1' style='color:#58151c;'>
                                            <i class='bi bi-exclamation-triangle-fill me-2'></i>
                                            <span class='small fw-bold'>Validation Failed</span>
                                        </div>

                                        <ul class='mb-0 ps-3 small'>
                                            {string.Join("", errors.Select(err => $"<li style='color:#58151c;'>{System.Net.WebUtility.HtmlEncode(err)}</li>"))}
                                        </ul>

                                        <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                                    </div>";

                    return Content(errorHtml, "text/html");
                }



                //await FillCreateLookupsAsync(model.BLId, cancellationToken);
                //return PartialView("_CreateDetentionModal", model);
            }


            var result = await _detentionService.CreateAndCalculateAsync(
                model,
                cancellationToken);

            HxTrigger(new
            {
                refreshDetentionsGrid = true,
                toastSuccess = "Detention Saved Successfully!!!"
            });
            return PartialView("_DetentionCalculationResult", result);
        }


        private async Task FillCreateLookupsAsync(Guid? blId, Guid? bookingId, CancellationToken cancellationToken)
        {
            ViewBag.Companies = (await _baseDataService.GetCompanies()).ToSelectListItems(x => x.Id.ToString(), x => $"{x.Person.PersonName}");
            if (blId != null && blId.HasValue && blId != Guid.Empty) { ViewBag.Parties = await _jobService.GetJobPartiesSelectListByBLIdAsync(blId.Value); }
            if (bookingId != null && bookingId.HasValue && bookingId != Guid.Empty) { ViewBag.Parties = await _jobService.GetJobPartiesSelectListByBookingIdAsync(bookingId.Value); }
            ViewBag.BillsOfLading = await _detentionService.GetBLSelectListAsync(blId);
            ViewBag.Bookings = await _detentionService.GetBookingSelectListAsync(bookingId);
            ViewBag.TariffBooks = (await _baseDataService.GetTariffBooksByTypeIdAsync(3)).ToSelectListItems(x => x.Id.ToString(), x => $"{x.Code} - {x.Name}");
            ViewBag.TradeTypes = (await _baseDataService.GetTradeTypesAsync()).ToSelectListItems(x => x.Id.ToString(), x => $"{x.TradeTypeName}");
            ViewBag.Currencies = (await _baseDataService.GetCurrenciesAsync()).ToSelectListItems(x => x.Id.ToString(), x => $"{x.CurrencyCode}");
            ViewBag.FinanceDirections = (await _baseDataService.GetFinanceDirectionsAsync()).ToSelectListItems(x => x.Id.ToString(), x => $"{x.InvoiceContextName}");
        }
        [HttpGet]
        public async Task<IActionResult> GetDetentionStartBaseModal(Guid? blId, Guid? bookingId, int companyId)
        {
            var model = await _detentionService.GetDetentionStartBaseAsync(blId, bookingId, companyId)
                        ?? new DetentionStartBaseViewModel
                        {
                            BLId = blId,
                            CompanyId = companyId,
                            DetentionStartBaseDateForBuy = DateTime.Today,
                            DetentionStartBaseDateForSell = DateTime.Today
                        };

            return PartialView("_DetentionStartBaseModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDetentionStartBase([FromForm] DetentionStartBaseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_DetentionStartBaseModalPartial", model);
            }

            try
            {
                await _detentionService.SaveDetentionStartBaseAsync(model);

                // Trigger an event to close the modal
                Response.Headers.Append("HX-Trigger", "closeModal");
                return Content("<div class='alert alert-success'>Dates saved successfully!</div>");
            }
            catch (Exception ex)
            {
                // Log exception here...
                ModelState.AddModelError("", "An error occurred while saving the data.");
                return PartialView("_DetentionStartBaseModalPartial", model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Print(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var pdfBytes = await _detentionService.GenerateDetentionReportPdfAsync(id, cancellationToken);
                var fileName = $"Detention-Statement-{id}.pdf";

                // باز شدن مستقیم درون تب مرورگر به صورت اینلاین
                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(pdfBytes, "application/pdf");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintFarsi(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var pdfBytes = await _detentionService.GenerateDetentionReportPdfFarsiAsync(id, cancellationToken);
                var fileName = $"Detention-Statement-{id}.pdf";

                // باز شدن مستقیم درون تب مرورگر به صورت اینلاین
                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(pdfBytes, "application/pdf");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> PrintStatement(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var pdfBytes = await _detentionService.GenerateDetentionStatementPdfAsync(id, cancellationToken);
                var fileName = $"DetentionStatement_{id:N}_{DateTime.Now:yyyyMMddHHmm}.pdf";

                // inline برای مشاهده در تب جدید مرورگر؛ در صورت نیاز به دانلود مستقیم از File(..., "application/pdf", fileName) استفاده کنید
                Response.Headers.Append("Content-Disposition", $"inline; filename={fileName}");
                return File(pdfBytes, "application/pdf");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {

            var detentionValidateToDelete = await _detentionService.ValidateToDeleteAsync(id);

            if (!detentionValidateToDelete)
            {
                HxTrigger(new
                {
                    toastFail = "Detention can not be deleted!!!!"
                });
            }
            else
            {
                var deleted = await _detentionService.DeleteAsync(id);
                if (deleted)
                {
                    HxTrigger(new
                    {
                        refreshDetentionsGrid = true,
                        toastSuccess = "Detention deletd successfully"
                    });
                }
            }
            return NoContent();
        }
    }
}
