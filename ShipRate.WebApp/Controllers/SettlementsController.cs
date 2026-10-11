using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;
using System.Data;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class SettlementsController : Controller
    {
        private readonly ISettlementService _settlementService;
        private readonly IPaymentDocumentService _paymentDocumentService;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPersonService _personService;
        private readonly IJobLookupService _joblookupService;

        public SettlementsController(
            ISettlementService settlementService,
            ApplicationDbContext context,
            ICurrentUserService currentUserService,
            IPaymentDocumentService paymentDocumentService,
            IPersonService personService,
            IJobLookupService joblookupService)
        {
            _settlementService = settlementService;
            _context = context;
            _currentUserService = currentUserService;
            _paymentDocumentService = paymentDocumentService;
            _personService = personService;
            _joblookupService = joblookupService;
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }



        [Authorize("Finance.Settlements.View")]
        public async Task<IActionResult> Index()
        {
            await FillLookupsAsync();
            return View(new SettlementSearchDto());
        }

        [HttpGet]
        [Authorize("Finance.Settlements.View")]
        public async Task<IActionResult> List(SettlementSearchDto input)
        {
            var result = await _settlementService.SearchAsync(input);
            return PartialView("_SettlementList", result);
        }

        [HttpGet]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> Create()
        {
            await FillLookupsAsync();

            var model = await _settlementService.GetCreateModelAsync();
            return PartialView("_CreateHeader", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> Create(SettlementHeaderCreateDto input)
        {
            if (!ModelState.IsValid)
            {
                await FillLookupsAsync();
                Response.StatusCode = 400;
                return PartialView("_CreateHeader", input);
            }

            var userId = _currentUserService.UserId;

            var id = await _settlementService.CreateHeaderAsync(input, userId);

            var model = await _settlementService.GetDetailsAsync(id);

            await FillLookupsAsync();

            Response.Headers["HX-Trigger"] = "settlementSaved";

            return PartialView("_Edit", model);
        }

        [HttpGet]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            await FillLookupsAsync();

            var model = await _settlementService.GetDetailsAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> UpdateHeader(SettlementHeaderEditDto input)
        {
            if (!ModelState.IsValid)
            {
                await FillLookupsAsync();
                Response.StatusCode = 400;
                return PartialView("_EditHeader", input);
            }

            await _settlementService.UpdateHeaderAsync(input, _currentUserService.UserId);

            var header = await _settlementService.GetHeaderForEditAsync(input.Id);

            await FillLookupsAsync();

            Response.Headers["HX-Trigger"] = "settlementSaved";

            return PartialView("_EditHeader", header);
        }

        [HttpGet]
        [Authorize("Finance.Settlements.Edit")]
        public IActionResult AddItem(Guid settlementId)
        {
            return PartialView("_AddItem", new SettlementItemCreateDto
            {
                SettlementId = settlementId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> AddItem(SettlementItemCreateDto input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return PartialView("_AddItem", input);
            }

            await _settlementService.AddItemAsync(input, _currentUserService.UserId);

            var items = await _settlementService.GetItemsAsync(input.SettlementId);

            Response.Headers["HX-Trigger"] = "settlementSaved";

            return PartialView("_Items", items);
        }

        [HttpPost]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> DeleteItem(Guid id, Guid settlementId)
        {
            await _settlementService.DeleteItemAsync(id);

            var items = await _settlementService.GetItemsAsync(settlementId);

            Response.Headers["HX-Trigger"] = "settlementSaved";

            return PartialView("_Items", items);
        }

        [HttpGet]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> AddPaymentAllocation(Guid settlementId)
        {
            var model = await _settlementService.GetPaymentAllocationForm(settlementId);

            return PartialView("_AddPaymentAllocation", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> AddPaymentAllocation(PaymentAllocationCreateDto input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return PartialView("_AddPaymentAllocation", input);
            }

            await _settlementService.AddPaymentAllocationAsync(input, _currentUserService.UserId);

            var items = await _settlementService.GetPaymentAllocationsAsync(input.SettlementId);

            Response.Headers["HX-Trigger"] = "{\"settlementSaved\":{\"closeModal\":true}}";

            return PartialView("_PaymentAllocations", items);
        }

        [HttpPost]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> DeletePaymentAllocation(Guid id, Guid settlementId)
        {
            await _settlementService.DeletePaymentAllocationAsync(id);

            var items = await _settlementService.GetPaymentAllocationsAsync(settlementId);

            Response.Headers["HX-Trigger"] = "settlementSaved";

            return PartialView("_PaymentAllocations", items);
        }

        private async Task FillLookupsAsync()
        {
            ViewBag.Companies = await _context.Companies
                .Include(x => x.Person)
                .Where(x => !x.IsDeleted && x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();
            ViewBag.Currencies = await _context.Currencies
                .Where(x => !x.IsDeleted && x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyName
                })
                .ToListAsync();

            ViewBag.Statuses = await _context.SettlementStatuses
                .Where(x => !x.IsDeleted && x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.SettlementStatusName
                })
                .ToListAsync();
        }
        [HttpGet]
        [Authorize("Finance.Settlements.View")]
        public async Task<IActionResult> JobSettlementDetails(Guid settlementItemId)
        {
            var model = await _settlementService.GetSettlementItemDetailsAsync(settlementItemId);
            return PartialView("~/Views/Settlements/_JobSettlementDetails.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.Settlements.Edit")]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var result = await _settlementService.ApproveSettlementAsync(id, userId, cancellationToken);

            if (!result.IsSuccess)
            {
                Response.StatusCode = 400;
                return Content(result.ErrorMessage ?? "Unable to approve settlement.");
            }
            // ارسال رویداد برای رفرش خودکار جدول لیست با HTMX
            Response.Headers["HX-Trigger"] = "settlementSaved";
            return Ok();
        }


        [HttpGet]
        [Authorize("Finance.Settlements.View")]
        public async Task<IActionResult> PrintReceipt(Guid id, [FromServices] IWebHostEnvironment env, CancellationToken cancellationToken)
        {
            // ۱. دریافت داده‌ها از لایه سرویس
            var reportData = await _settlementService.GetSettlementReceiptReportDataAsync(id, cancellationToken);
            if (reportData == null)
                return NotFound();

            // ۲. حل مسیر فیزیکی لوگوی شرکت
            var logoPhysicalPath = Path.Combine(env.WebRootPath, "logos", $"logo-{reportData.CompanyId}.png");
            if (!System.IO.File.Exists(logoPhysicalPath))
            {
                logoPhysicalPath = Path.Combine(env.WebRootPath, "logos", $"logo-{reportData.CompanyId}.jpg");
                if (!System.IO.File.Exists(logoPhysicalPath))
                {
                    logoPhysicalPath = Path.Combine(env.WebRootPath, "images", "default-logo.png");
                }
            }
            reportData.CompanyLogoPath = System.IO.File.Exists(logoPhysicalPath) ? logoPhysicalPath : string.Empty;

            // ۳. ساخت DataTable برای هدر و اطلاعات کلی (Settlement)
            var dtHeader = new DataTable("Settlement");
            dtHeader.Columns.Add("CompanyName", typeof(string));
            dtHeader.Columns.Add("CompanyLogoPath", typeof(string));
            dtHeader.Columns.Add("SettlementNo", typeof(int));
            dtHeader.Columns.Add("SettlementDate", typeof(string));
            dtHeader.Columns.Add("PartyName", typeof(string));
            dtHeader.Columns.Add("CurrencyName", typeof(string));
            dtHeader.Columns.Add("StatusName", typeof(string));
            dtHeader.Columns.Add("Remarks", typeof(string));
            dtHeader.Columns.Add("TotalAmount", typeof(decimal));
            dtHeader.Columns.Add("TotalAllocated", typeof(decimal));
            dtHeader.Columns.Add("RemainingAmount", typeof(decimal));

            dtHeader.Rows.Add(
                reportData.CompanyName,
                reportData.CompanyLogoPath,
                reportData.SettlementNo,
                reportData.SettlementDate,
                reportData.PartyName,
                reportData.CurrencyName,
                reportData.StatusName,
                reportData.Remarks,
                reportData.TotalAmount,
                reportData.TotalAllocated,
                reportData.RemainingAmount
            );

            // ۴. ساخت DataTable برای اقلام تسویه (Items)
            var dtItems = new DataTable("Items");
            dtItems.Columns.Add("RowIndex", typeof(int));
            dtItems.Columns.Add("InvoiceNo", typeof(string));
            dtItems.Columns.Add("ChargeTitle", typeof(string));
            dtItems.Columns.Add("Amount", typeof(decimal));
            dtItems.Columns.Add("CoveredAmount", typeof(decimal));
            dtItems.Columns.Add("RemainingAmount", typeof(decimal));
            dtItems.Columns.Add("Remarks", typeof(string));

            foreach (var item in reportData.Items)
            {
                dtItems.Rows.Add(
                    item.RowIndex,
                    item.InvoiceNo ?? "-",
                    item.ChargeTitle ?? "-",
                    item.Amount,
                    item.CoveredAmount,
                    item.RemainingAmount,
                    item.Remarks ?? string.Empty
                );
            }

            // ۵. ساخت DataTable برای اسناد پرداخت و تخصیص‌ها (Payments)
            var dtPayments = new DataTable("Payments");
            dtPayments.Columns.Add("RowIndex", typeof(int));
            dtPayments.Columns.Add("DocumentNo", typeof(string));
            dtPayments.Columns.Add("DocumentDate", typeof(string));
            dtPayments.Columns.Add("PaymentMethod", typeof(string));
            dtPayments.Columns.Add("AllocatedAmount", typeof(decimal));
            dtPayments.Columns.Add("SettledInItemCurrency", typeof(decimal));
            dtPayments.Columns.Add("ExchangeRate", typeof(decimal));
            dtPayments.Columns.Add("Remarks", typeof(string));

            foreach (var pay in reportData.Payments)
            {
                dtPayments.Rows.Add(
                    pay.RowIndex,
                    pay.DocumentNo ?? "-",
                    pay.DocumentDate ?? "-",
                    pay.PaymentMethod ?? "-",
                    pay.AllocatedAmount,
                    pay.SettledInItemCurrency,
                    pay.ExchangeRate,
                    pay.Remarks ?? string.Empty
                );
            }

            // ۶. قرار دادن جداول در DataSet
            var dataSet = new DataSet("SettlementDataSet");
            dataSet.Tables.Add(dtHeader);
            dataSet.Tables.Add(dtItems);
            dataSet.Tables.Add(dtPayments);

            // ۷. بارگذاری و اجرای FastReport
            var reportFilePath = Path.Combine(env.ContentRootPath, "Reports", "Finance", "SettlementReceipt.frx");
            if (!System.IO.File.Exists(reportFilePath))
            {
                return BadRequest($"Report template file not found at: {reportFilePath}");
            }

            using var report = new Report();
            report.Load(reportFilePath);

            // ثبت داده‌ها در گزارش
            report.RegisterData(dataSet, "SettlementDataSet");

            // فعال‌سازی صریح DataSources
            report.GetDataSource("Settlement")?.Init();
            report.GetDataSource("Items")?.Init();
            report.GetDataSource("Payments")?.Init();

            report.Prepare();

            // خروجی PDF
            using var pdfExport = new PDFSimpleExport();
            var stream = new MemoryStream();
            pdfExport.Export(report, stream);
            stream.Position = 0;

            //return File(stream, "application/pdf", $"Settlement_Receipt_{reportData.SettlementNo}.pdf");
            return File(stream, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> PrintView(Guid id)
        {
            var reportData = await _settlementService.GetSettlementReceiptReportDataAsync(id);
            if (reportData == null)
            {
                return NotFound();
            }

            return View("SettlementReceiptPrint", reportData);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            {
                var settlement = await _context.Settlements
                    .Include(x => x.Items)
                    .Include(x => x.PaymentAllocations)
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                //if (settlement == null) return BadRequest();

                var invoiceIds = settlement.Items
                    .Where(x => x.InvoiceId.HasValue)
                    .Select(x => x.InvoiceId!.Value)
                    .Distinct()
                    .ToList();

                var paymentDocumentIds = settlement.PaymentAllocations
                    .Select(x => x.PaymentDocumentId)
                    .Distinct()
                    .ToList();

                var detentionIds = settlement.Items
                    .Where(x => x.DetentionId.HasValue)
                    .Select(x => x.DetentionId!.Value)
                    .Distinct()
                    .ToList();



                settlement.IsDeleted = true;
                settlement.DeletedAt = DateTime.Now;
                settlement.StatusId = 10;
                settlement.DeleteUserId = _currentUserService.UserId;
                _context.SaveChanges();
                await _settlementService.SyncSettlementBalancesAsync(invoiceIds, paymentDocumentIds, detentionIds);
                await transaction.CommitAsync();
                HxTrigger(new
                {
                    settlementSaved = true,
                    toastSuccess = "Settlement Deleted Succesfully"
                });
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> OpenSettlementModal([FromBody] OpenSettlementModalRequest request)
        {
            var model = new SettlementModalViewModel
            {
                SettlementDate = DateTime.Today,
                BLId = request.BLId ?? Guid.Empty,
                PartyId = request.PartyId
            };

            var availableItems = new List<PendingSettlementItemDto>();

            // ۱. دریافت فاکتورهای انتخاب‌شده
            if (request.InvoiceIds != null && request.InvoiceIds.Any())
            {
                var invoices = await _settlementService.GetInvoicesForSettlementAsync(request.InvoiceIds);
                foreach (var inv in invoices)
                {
                    availableItems.Add(new PendingSettlementItemDto
                    {
                        SourceId = inv.Id,
                        ItemType = "Invoice",
                        DocumentNo = inv.InvoiceNo,
                        TotalAmount = inv.TotalAmount,
                        RemainingAmount = inv.RemainingAmount,
                        CurrencyId = inv.CurrencyId,
                        CurrencyCode = inv.CurrencyCode,
                        CompanyId = inv.CompanyId
                    });
                }

                // در صورتی که PartyId داده نشده باشد، از اولین فاکتور خوانده شود
                if (!model.PartyId.HasValue && invoices.Any())
                {
                    model.PartyId = invoices.First().PartyId;
                    model.PartyName = invoices.First().PartyName;
                    model.CurrencyId = invoices.First().CurrencyId;
                }
            }

            //// ۲. دریافت دموراژها/Detentionهای انتخاب‌شده
            //if (request.DetentionIds != null && request.DetentionIds.Any())
            //{
            //    var detentions = await _settlementService.GetDetentionsForSettlementAsync(request.DetentionIds);
            //    foreach (var det in detentions)
            //    {
            //        availableItems.Add(new PendingSettlementItemDto
            //        {
            //            SourceId = det.Id,
            //            ItemType = "Detention",
            //            DocumentNo = det.ReferenceNo ?? $"DET-{det.Id.ToString()[..8]}",
            //            TotalAmount = det.TotalAmount,
            //            RemainingAmount = det.RemainingAmount,
            //            CurrencyId = det.CurrencyId,
            //            CurrencyCode = det.CurrencyCode
            //        });
            //    }

            //    if (!model.PartyId.HasValue && detentions.Any())
            //    {
            //        model.PartyId = detentions.First().PartyId;
            //        model.PartyName = detentions.First().PartyName;
            //    }
            //}

            if (availableItems.Select(x => x.CompanyId).Distinct().Count() > 1)
            {
                //Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_AlertError", "Multiple Companies not Allowd.");

            }
            else
            {
                model.CompanyId = availableItems.Select(x => x.CompanyId).First();
            }





            model.AvailableItems = availableItems;
            model.AvailablePaymentDocuments = await _paymentDocumentService.GetAvailablePaymentDocsByPartyAsync(model.PartyId.Value);

            // ۴. پر کردن لیست‌های انتخابی (Dropdowns)
            var currencies = await _joblookupService.GetCurrenciesAsync();
            model.Currencies = new SelectList(currencies, "Value", "Text");
            var party = await _personService.GetByIdAsync(model.PartyId.Value);
            var partyItems = new List<SelectListItem>
                                {
                                    new SelectListItem
                                    {
                                        Value = model.PartyId.ToString(),
                                        Text = party.PersonName,
                                        Selected = true
                                    }
                                };
            model.Parties = new SelectList(partyItems, "Value", "Text", model.PartyId.ToString());
            var companies = await _joblookupService.GetCompaniesAsync();
            model.Companies = new SelectList(companies, "Value", "Text");

            return PartialView("_SettlementModal", model);
        }
        [HttpGet]
        public async Task<IActionResult> OpenBLSettlementModal(Guid blId)
        {
            var bl = await _context.BLs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == blId);

            if (bl == null)
                return NotFound(new { message = "B/L record not found." });

            // 1. Fetch pending invoices with remaining balance > 0
            var pendingInvoices = await _context.Invoices
                .Where(i => i.BLId == blId && !i.IsDeleted && i.TotalAmount > i.SettledAmount)
                .Select(i => new PendingSettlementItemDto
                {
                    SourceId = i.Id,
                    ItemType = "Invoice",
                    DocumentNo = i.SerialDisplay,
                    TotalAmount = i.TotalAmount,
                    RemainingAmount = i.RemainingAmount,
                    CurrencyId = i.CurrencyId,
                    CurrencyCode = i.Currency.CurrencyCode
                })
                .ToListAsync();

            // 2. Fetch pending detentions with remaining balance > 0
            var pendingDetentions = await _context.Detentions
                .Where(d => d.BLId == blId && !d.IsDeleted && d.DetentionStatusId == 1 && d.FinalPayableAmount > d.SettledAmount)
                .Select(d => new PendingSettlementItemDto
                {
                    SourceId = d.Id,
                    ItemType = "Detention",
                    DocumentNo = "DET-" + d.DetentionNo.ToString(),
                    TotalAmount = d.FinalPayableAmount,
                    RemainingAmount = d.RemainingAmount,
                    CurrencyId = d.FinalCurrencyId,
                    CurrencyCode = d.PaymentCurrency.CurrencyCode
                })
                .ToListAsync();
            var availableItems = pendingInvoices.Concat(pendingDetentions).ToList();

            //if (availableItems.Select(x => x.CompanyId).Distinct().Count() > 1)
            //{
            //    return PartialView("_AlertError", "Multiple Companies not Allowd.");

            //}


            //var companyId = availableItems.Select(x => x.CompanyId).First();

            // 3. Fetch available payment documents for this BL (or related party) with unallocated balance > 0
            var availablePaymentDocs = await _context.PaymentDocuments
                .Where(p => p.Amount > p.AllocatedAmount)
                .Select(p => new AvailablePaymentDocDto
                {
                    PaymentDocumentId = p.Id,
                    DocumentNo = p.DocumentNo,
                    DocumentDate = p.DocumentDate,
                    TotalAmount = p.Amount,
                    UnallocatedAmount = p.UnallocatedAmount,
                    CurrencyId = p.CurrencyId,
                    CurrencyCode = p.Currency.CurrencyCode,
                    PaymentMethodName = p.PaymentMethod.PaymentMethodName
                })
                .ToListAsync();

            // 4. Dropdowns
            var parties = await _context.JobParties
                .Where(jp => jp.JobId == bl.JobId)
                .Select(jp => new SelectListItem
                {
                    Value = jp.PartyId.ToString(),
                    Text = jp.Party.PersonName
                })
                .Distinct()
                .ToListAsync();
            var companies = await _context.Companies
                .Select(jp => new SelectListItem
                {
                    Value = jp.Id.ToString(),
                    Text = jp.CompanyAbb
                })
                .Distinct()
                .ToListAsync();

            var currencies = await _context.Currencies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CurrencyCode
                })
                .ToListAsync();

            var viewModel = new SettlementModalViewModel
            {
                BLId = bl.Id,
                BLNo = bl.BLNo,
                AvailableItems = availableItems,
                AvailablePaymentDocuments = availablePaymentDocs,
                Parties = new SelectList(parties, "Value", "Text"),
                Currencies = new SelectList(currencies, "Value", "Text"),
                Companies = new SelectList(companies, "Value", "Text")
            };
            return PartialView("~/Views/Settlements/_SettlementModal.cshtml", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OpenBookingSettlementModal(Guid bookingId)
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            if (booking == null)
                return NotFound(new { message = "Booking record not found." });

            // 1. Fetch pending invoices with remaining balance > 0
            var pendingInvoices = await _context.Invoices
                .Where(i => i.BookingId == bookingId && !i.IsDeleted && i.TotalAmount > i.SettledAmount)
                .Select(i => new PendingSettlementItemDto
                {
                    SourceId = i.Id,
                    ItemType = "Invoice",
                    DocumentNo = i.SerialDisplay,
                    TotalAmount = i.TotalAmount,
                    RemainingAmount = i.RemainingAmount,
                    CurrencyId = i.CurrencyId,
                    CurrencyCode = i.Currency.CurrencyCode
                })
                .ToListAsync();

            // 2. Fetch pending detentions with remaining balance > 0
            var pendingDetentions = await _context.Detentions
                .Where(d => d.BookingId == bookingId && !d.IsDeleted && d.DetentionStatusId == 1 && d.FinalPayableAmount > d.SettledAmount)
                .Select(d => new PendingSettlementItemDto
                {
                    SourceId = d.Id,
                    ItemType = "Detention",
                    DocumentNo = "DET-" + d.DetentionNo.ToString(),
                    TotalAmount = d.FinalPayableAmount,
                    RemainingAmount = d.RemainingAmount,
                    CurrencyId = d.FinalCurrencyId,
                    CurrencyCode = d.PaymentCurrency.CurrencyCode
                })
                .ToListAsync();
            var availableItems = pendingInvoices.Concat(pendingDetentions).ToList();

            //if (availableItems.Select(x => x.CompanyId).Distinct().Count() > 1)
            //{
            //    return PartialView("_AlertError", "Multiple Companies not Allowd.");

            //}


            //var companyId = availableItems.Select(x => x.CompanyId).First();

            // 3. Fetch available payment documents for this BL (or related party) with unallocated balance > 0
            var availablePaymentDocs = await _context.PaymentDocuments
                .Where(p => p.Amount > p.AllocatedAmount)
                .Select(p => new AvailablePaymentDocDto
                {
                    PaymentDocumentId = p.Id,
                    DocumentNo = p.DocumentNo,
                    DocumentDate = p.DocumentDate,
                    TotalAmount = p.Amount,
                    UnallocatedAmount = p.UnallocatedAmount,
                    CurrencyId = p.CurrencyId,
                    CurrencyCode = p.Currency.CurrencyCode,
                    PaymentMethodName = p.PaymentMethod.PaymentMethodName
                })
                .ToListAsync();

            // 4. Dropdowns
            var parties = await _context.JobParties
                .Where(jp => jp.JobId == booking.JobId)
                .Select(jp => new SelectListItem
                {
                    Value = jp.PartyId.ToString(),
                    Text = jp.Party.PersonName
                })
                .Distinct()
                .ToListAsync();
            var companies = await _context.Companies
                .Select(jp => new SelectListItem
                {
                    Value = jp.Id.ToString(),
                    Text = jp.CompanyAbb
                })
                .Distinct()
                .ToListAsync();

            var currencies = await _context.Currencies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CurrencyCode
                })
                .ToListAsync();

            var viewModel = new SettlementModalViewModel
            {
                BookingId = booking.Id,
                BookingNo = booking.BookingNo,
                AvailableItems = availableItems,
                AvailablePaymentDocuments = availablePaymentDocs,
                Parties = new SelectList(parties, "Value", "Text"),
                Currencies = new SelectList(currencies, "Value", "Text"),
                Companies = new SelectList(companies, "Value", "Text")
            };
            return PartialView("~/Views/Settlements/_SettlementModal.cshtml", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SaveSettlement([FromBody] SaveSettlementRequestDto request)
        {
            if (request == null || !request.Allocations.Any())
                return Json(new { success = false, message = "Invalid settlement payload or empty allocations." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var settlement = new Settlement
                {
                    Id = Guid.CreateVersion7(),
                    PartyId = request.PartyId,
                    CurrencyId = request.CurrencyId,
                    SettlementDate = request.SettlementDate,
                    Remarks = request.Remarks,
                    StatusId = 1,
                    CreateUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.Now,
                    CompanyId = request.CompanyId
                };
                if (request.BLId.HasValue && request.BLId != Guid.Empty) settlement.BLId = request.BLId;
                if (request.BookingId.HasValue && request.BookingId != Guid.Empty) settlement.BookingId = request.BookingId;


                //کنترل واجب و ضروری باید اینجا باشد برای اینکه دو  تا شرکت باهم قاطی نشوند 
                //خیلی مهم 
                //خیلی مهم 
                //خیلی مهم 

                var groupedBySource = request.Allocations.GroupBy(x => new { x.ItemType, x.SourceId });
                foreach (var group in groupedBySource)
                {
                    var settlementItem = new SettlementItem
                    {
                        Id = Guid.CreateVersion7(),
                        SettlementId = settlement.Id,
                        InvoiceId = group.Key.ItemType == "Invoice" ? group.Key.SourceId : null,
                        DetentionId = group.Key.ItemType == "Detention" ? group.Key.SourceId : null,
                        Amount = group.Sum(x => x.SettledAmountInItemCurrency),
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId,
                        Remarks = "-"
                    };

                    foreach (var alloc in group)
                    {
                        var paymentAllocation = new SettlementItemPaymentAllocation
                        {
                            Id = Guid.CreateVersion7(),
                            SettlementId = settlement.Id,
                            SettlementItemId = settlementItem.Id,
                            PaymentDocumentId = alloc.PaymentDocumentId,
                            AllocatedAmount = alloc.AllocatedAmountFromDoc,
                            SettledAmountInItemCurrency = alloc.SettledAmountInItemCurrency,
                            ExchangeRateValue = alloc.ExchangeRateValue,
                            CreateUserId = _currentUserService.UserId,
                            CreatedAt = DateTime.Now,
                            Remarks = "-"
                        };
                        settlementItem.PaymentAllocations.Add(paymentAllocation);
                    }
                    settlement.Items.Add(settlementItem);
                }
                _context.Settlements.Add(settlement);
                await _context.SaveChangesAsync();

                await _settlementService.SyncSettlementBalancesAsync(
                    request.Allocations.Where(x => x.ItemType == "Invoice").Select(x => x.SourceId).Distinct(),
                    request.Allocations.Select(x => x.PaymentDocumentId).Distinct(),
                    request.Allocations.Where(x => x.ItemType == "Detention").Select(x => x.SourceId).Distinct()
                );

                await transaction.CommitAsync();
                return Json(new { success = true, settlementId = settlement.Id, message = "Settlement completed successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}