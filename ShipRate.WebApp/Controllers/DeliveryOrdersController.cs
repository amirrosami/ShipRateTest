using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DeliveryOrdersController : Controller
    {
        private readonly IDeliveryOrderService _deliveryOrderService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _webhostEnv;

        public DeliveryOrdersController(IDeliveryOrderService deliveryOrderService, ICurrentUserService currentUserService, IWebHostEnvironment webhostEnv)
        {
            _deliveryOrderService = deliveryOrderService;
            _currentUserService = currentUserService;
            _webhostEnv = webhostEnv;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }



        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.View")]
        public async Task<IActionResult> Index(
            [FromQuery] DeliveryOrderSearchFilter filter,
            CancellationToken cancellationToken)
        {
            // مقداردهی پیش‌فرض Paging
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var model = await _deliveryOrderService.GetPagedListAsync(filter, cancellationToken);

            // اگر درخواست از سمت HTMX بود، فقط گرید/جدول بروزرسانی شود
            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return PartialView("_ListGrid", model);
            }
            return View(model);
        }

        //[HttpGet]
        //[Authorize("ImportSpecific.DeliveryOrders.Edit")]
        //public async Task<IActionResult> Create(CancellationToken cancellationToken)
        //{
        //    var vm = await _deliveryOrderService.GetCreatePageAsync(null, cancellationToken);
        //    return View(vm);
        //}

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> CreatePartial(Guid? blId, CancellationToken cancellationToken)
        {
            var vm = await _deliveryOrderService.GetCreatePageAsync(blId, cancellationToken);
            vm.BLId = blId;
            return PartialView("_Create", vm);
        }
        //[HttpGet]
        //[Authorize("ImportSpecific.DeliveryOrders.Edit")]
        //public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }

        //    var vm = await _deliveryOrderService.GetEditPageVmAsync(id, cancellationToken);
        //    if (vm == null)
        //    {
        //        return NotFound();
        //    }

        //    // تنظیم عنوان و استفاده از همان ویوی Create
        //    ViewData["Title"] = "Edit Delivery Order";
        //    return View("Create", vm);
        //}
        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }
            var vm = await _deliveryOrderService.GetEditPageVmAsync(id, cancellationToken);
            if (vm == null)
            {
                return NotFound();
            }
            if (vm.StatusId != 10)
            {
                HxTrigger(
                    new
                    {
                        toastFail = "Delivey Order Status in not draft!!!"
                    }
                    );
                return BadRequest();
            }
            // تنظیم عنوان و استفاده از همان ویوی Create
            ViewData["Title"] = "Edit Delivery Order";
            return PartialView("_Create", vm);
        }
        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> BLSearchModal(CancellationToken cancellationToken)
        {
            var items = await _deliveryOrderService.SearchBLAsync(null, cancellationToken);
            return PartialView("_BLSearchModal", items);
        }

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> SearchBL(string? term, CancellationToken cancellationToken)
        {
            var items = await _deliveryOrderService.SearchBLAsync(term, cancellationToken);
            return PartialView("_BLSearchResults", items);
        }

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> BLSummary(Guid blId, CancellationToken cancellationToken)
        {
            var vm = await _deliveryOrderService.GetBLSummaryAsync(blId, cancellationToken);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> SaveDraft(CreateDeliveryOrderPageVm model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await _deliveryOrderService.FillCreatePageLookupsAsync(model, cancellationToken);

                return PartialView("_SaveDraftResult", new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "Please complete the required fields."
                });
            }

            var result = await _deliveryOrderService.SaveDraftAsync(new SaveDeliveryOrderDraftRequestVm
            {
                DeliveryOrderId = model.DeliveryOrderId,
                BLId = model.BLId,
                DONo = model.DONo,
                DoDate = model.DoDate,
                ExpiryDate = model.ExpiryDate,
                ArrivalDate = model.ArrivalDate,
                ClearanceTypeId = model.ClearanceTypeId,
                BLReleaseTypeId = model.BLReleaseTypeId,
                OriginalBLCount = model.OriginalBLCount,
                Remarks = model.Remarks,
                ExternalIssuerId = model.ExternalIssuerId,
                CarrierAgentId = model.CarrierAgentId,
                CarrierId = model.CarrierId,
                ExternalIssuerText = model.ExternalIssuerText,
                ReceivedAtDateTime = model.ReceivedAtDateTime,
                ReceivedByUserId = model.ReceivedByUserId,
                ReceiveMethodId = model.ReceiveMethodId,
                TIPId = model.TIPId,
                CompanyId = 1,
                DOOperationalModeId = model.DOOperationalModeId.Value
            }, cancellationToken);

            HxTrigger(
                new
                {
                    refreshDOGrid = true,
                    toastSuccess = "Saved Sussefully"
                }
                );


            return PartialView("_SaveDraftResult", result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> Finalize(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        {
            var result = await _deliveryOrderService.FinalizeAsync(request, cancellationToken);
            return PartialView("_FinalizeResult", result);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize("ImportSpecific.DeliveryOrders.Approve")]
        //public async Task<IActionResult> Approve(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        //{
        //    var result = await _deliveryOrderService.ApproveAsync(request, cancellationToken);
        //    return PartialView("_FinalizeResult", result);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.DeliveryOrders.Release")]
        public async Task<IActionResult> Release(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        {
            var result = await _deliveryOrderService.ReleaseAsync(request, cancellationToken);
            return PartialView("_FinalizeResult", result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> Collect(Guid id, CancellationToken cancellationToken)
        {
            var request = new FinalizeDeliveryOrderRequestVm();
            request.DeliveryOrderId = id;
            var result = await _deliveryOrderService.CollectedAsync(request, cancellationToken);
            if (result.Success)
            {
                HxTrigger(
                    new
                    {
                        refreshDOGrid = true,
                        toastSuccess = "DO Delivered Sussefully!!!!"
                    }
                    );
            }
            //return PartialView("_FinalizeResult", result);
            return NoContent();
        }
        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.View")]
        public async Task<IActionResult> Details(
            Guid id,
            CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var vm = await _deliveryOrderService.GetDetailsAsync(id, cancellationToken);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> Print(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var vm = await _deliveryOrderService.GetDetailsAsync(id, cancellationToken);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        [HttpGet]
        public async Task<IActionResult> PrintPDF(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var vm = await _deliveryOrderService.GetDetailsAsync(id, cancellationToken);
            if (vm == null)
            {
                return NotFound();
            }

            var frxPath = Path.Combine(_webhostEnv.ContentRootPath, "Reports", "DO", "DO.frx");
            if (!System.IO.File.Exists(frxPath))
            {
                return NotFound("قالب گزارش یافت نشد.");
            }

            // فراخوانی متد سرویس
            var pdfBytes = _deliveryOrderService.GenerateDeliveryOrderPdf(vm, frxPath);
            var fileName = $"DO_{(string.IsNullOrWhiteSpace(vm.DONo) ? id.ToString() : vm.DONo)}.pdf";
            //return File(pdfBytes, "application/pdf", fileName);
            return File(pdfBytes, "application/pdf");
        }
        [HttpDelete]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> Delete(Guid id,
            [FromQuery] DeliveryOrderSearchFilter filter,
            CancellationToken cancellationToken)
        {
            try
            {
                var success = await _deliveryOrderService.DeleteDraftAsync(id, cancellationToken);
                if (!success)
                {
                    return NotFound("Delivery Order not found.");
                }

                HxTrigger(
                    new
                    {
                        refreshDOGrid = true,
                        toastSuccess = "DO Deleted Sussefully!!!!"
                    }
                    );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }





            // مقداردهی مجدد پارامترهای فیلتر برای رندر کردن مجدد گرید
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            var model = await _deliveryOrderService.GetPagedListAsync(filter, cancellationToken);

            // بازگرداندن گرید به‌روزرسانی شده بدون رکورد حذف شده
            return PartialView("_ListGrid", model);
        }




        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> GetCancelModal(Guid id, CancellationToken cancellationToken)
        {
            var doEntity = await _deliveryOrderService.GetDetailsAsync(id, cancellationToken);
            if (doEntity == null)
            {
                return NotFound("Delivery Order not found.");
            }

            var model = new CancelDeliveryOrderVm
            {
                DeliveryOrderId = doEntity.DeliveryOrderId,
                DONo = doEntity.DONo,
                BLNo = doEntity.BLSummary.BLNo
            };

            return PartialView("_CancelModalContent", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> Cancel(CancelDeliveryOrderVm model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CancelModalContent", model);
            }

            var success = await _deliveryOrderService.CancelDeliveryOrderAsync(
                model.DeliveryOrderId,
                model.CancellationReason,
                cancellationToken);

            if (!success)
            {
                return BadRequest("Unable to cancel the Delivery Order.");
            }

            // بستن مودال و ریلود کردن کل گرید داشبورد با ارسال هدر HTMX
            Response.Headers.Add("HX-Trigger", "refreshGrid");
            return Content("<div class='alert alert-success'>Delivery Order cancelled successfully.</div>");
        }



        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.View")]
        public async Task<IActionResult> GetDetentionsGrid(Guid id, CancellationToken cancellationToken)
        {
            var vm = await _deliveryOrderService.GetDetentionsByBlAsync(id, cancellationToken);

            if (vm == null)
            {
                return PartialView("_DetentionsGrid", new DeliveryOrderDetentionsVm
                {
                    DeliveryOrderId = id
                });
            }

            return PartialView("_DetentionsGrid", vm);
        }


        [HttpGet]
        public async Task<IActionResult> DocumentPanel(Guid deliveryOrderId, CancellationToken cancellationToken)
        {
            var vm = await _deliveryOrderService.BuildDocumentPanelAsync(deliveryOrderId, cancellationToken);
            return PartialView("_DocumentPanel", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> UploadDocumentFiles(
            Guid deliveryOrderId,
            int documentTypeId,
            List<IFormFile> files,
            CancellationToken cancellationToken)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("No files selected for upload.");
            }

            // مقادیر فرضی احراز هویت کاربری (مطابق با ساختار پیاده‌سازی شده در پروژه شما)
            var currentCompanyId = 1; // یا مقادیر معادل آن از Identity/Claims
            var currentUserId = _currentUserService.UserId;

            try
            {
                var cardVm = await _deliveryOrderService.UploadDocumentFilesAsync(
                    deliveryOrderId,
                    documentTypeId,
                    files,
                    currentCompanyId,
                    currentUserId,
                    cancellationToken);



                return PartialView("_DocumentUploadCard", cardVm);
            }
            catch (Exception ex)
            {
                // در صورت بروز خطا لاگ ثبت شده و پاسخ مناسب بازگردانده می‌شود
                //_logger.LogError(ex, "Error uploading document attachments for DO: {DeliveryOrderId}", deliveryOrderId);
                return StatusCode(500, "An error occurred while uploading files.");
            }
        }






        //[HttpGet]
        //public async Task<IActionResult> DownloadDocumentAttachment(Guid id, CancellationToken cancellationToken)
        //{
        //    if (id == Guid.Empty)
        //    {
        //        return BadRequest();
        //    }

        //    // دریافت اطلاعات فایل از لایه سرویس
        //    var attachment = await _deliveryOrderService.GetAttachmentByIdAsync(id, cancellationToken);
        //    if (attachment == null || attachment.Content == null)
        //    {
        //        return NotFound("File not found.");
        //    }

        //    // بازگرداندن فایل به مرورگر جهت دانلود یا نمایش مستقیم
        //    return File(
        //        fileContents: attachment.Content,
        //        contentType: attachment.ContentType ?? "application/octet-stream",
        //        fileDownloadName: attachment.FileName
        //    );
        //}

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.View")]
        public async Task<IActionResult> DownloadDocumentAttachment(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty) return BadRequest();

            var attachment = await _deliveryOrderService.GetAttachmentByIdAsync(id, cancellationToken);

            if (attachment == null || attachment.Content == null)
                return NotFound("File not found.");

            // حذف پارامتر fileDownloadName برای جلوگیری از دانلود اجباری
            // این کار باعث می‌شود هدر Content-Disposition برابر با inline قرار گیرد
            return File(attachment.Content, attachment.ContentType ?? "application/octet-stream");
        }
        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> GetBlCharges(Guid blId)
        {
            // دریافت هزینه‌های بارنامه از سرویس
            var charges = await _deliveryOrderService.GetChargesByBlIdAsync(blId);

            return PartialView("_BlChargesGrid", charges);
        }

        [HttpGet]
        [Authorize("ImportSpecific.DeliveryOrders.Edit")]
        public async Task<IActionResult> GetBlInvoices(Guid blId)
        {
            var invoices = await _deliveryOrderService.GetInvoicesByBlIdAsync(blId);
            return PartialView("_BlInvoicesGrid", invoices);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var result = await _deliveryOrderService.ApproveAsync(id, userId, cancellationToken);
            if (!result)
            {
                Response.StatusCode = 400;
                return Content("Unable to approve settlement.");
            }
            HxTrigger(
                new
                {
                    refreshDOGrid = true,
                    toastSuccess = "DO Approved Sussefully"
                }
                );
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> FreightLetterModal(Guid doId, CancellationToken ct)
        {
            var vm = await _deliveryOrderService.GetFreightLetterUpsertVmAsync(doId, ct);
            return PartialView("_FreightLetterModal", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveFreightLetter(FreightLetterUpsertVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return PartialView("_FreightLetterModal", vm);

            var saved = await _deliveryOrderService.UpsertFreightLetterAsync(vm, ct);

            // بعد از ذخیره، همان فرم را دوباره برگردان تا Print فعال شود
            saved.IsSaved = true;

            // اختیاری: برای نمایش toast یا بستن/باز نکردن مودال
            Response.Headers["HX-Trigger"] = "freightLetterSaved";

            return PartialView("_FreightLetterModal", saved);
        }


        [HttpGet]
        public async Task<IActionResult> PrintFreightLetter(Guid doId, CancellationToken ct)
        {
            var templatePath = Path.Combine(_webhostEnv.ContentRootPath, "Reports", "DO", "FreightLetter.frx");

            // اگر فایل در مسیر روت پروژه است:
            // var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Reports", "FreightLetter.frx");

            var pdfBytes = await _deliveryOrderService.GenerateFreightLetterPdfAsync(doId, templatePath, ct);

            // باز شدن مستقیم در تب مرورگر (inline)
            Response.Headers.Append("Content-Disposition", $"inline; filename=FreightLetter-{doId}.pdf");
            return File(pdfBytes, "application/pdf");
        }

    }
}
