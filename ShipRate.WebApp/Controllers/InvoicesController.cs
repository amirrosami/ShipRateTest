using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Interfaces.Reports;
using System.Text.Json;
namespace ShipRate.WebApp.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class InvoicesController : Controller
{
    private readonly IInvoiceAppService _invoiceAppService;
    private readonly IReportService _reportService;
    private readonly ITransportDocumentService _transportDocumentService;
    public InvoicesController(IInvoiceAppService invoiceAppService, IReportService reportService, ITransportDocumentService transportDocumentService)
    {
        _invoiceAppService = invoiceAppService;
        _reportService = reportService;
        _transportDocumentService = transportDocumentService;
    }
    private void HxTrigger(object triggers)
    {
        Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
    }
    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetListPageAsync(new InvoiceListFilterVm(), cancellationToken);
        return View(vm);
    }
    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> List([FromQuery] InvoiceListFilterVm filter, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetListPageAsync(filter, cancellationToken);

        if (filter.JobId != null && filter.JobId != Guid.Empty)
            return PartialView("_InvoiceGrid", vm);
        else
            return PartialView("_InvoiceList", vm);
    }

    [HttpGet]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> CreateModal(Guid? JobId, Guid? blId, Guid bookingId, CancellationToken cancellationToken, string filterType = "All")
    {
        var vm = await _invoiceAppService.GetCreateVmAsync(JobId, blId, bookingId, cancellationToken, filterType);
        vm.JobId = JobId;
        vm.CompanyId = 1;
        vm.DefaultTaxPercent = 10;
        if (JobId.HasValue && JobId != Guid.Empty)
        {
        }
        return PartialView("~/Views/Invoices/_InvoiceEditModal.cshtml", vm);
    }

    [HttpGet]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> CreateModalDO(Guid? JobId, Guid? BlId, CancellationToken cancellationToken)
    {
        if (BlId == null || BlId == Guid.Empty) return BadRequest();
        var iscntBL = await _transportDocumentService.BLHasCntAsync(BlId.Value);
        if (!iscntBL)
        {

            HxTrigger(
                new
                {
                    toastFail = "BL has not any Container!!!!"
                }
                );
            return BadRequest();
        }

        var vm = await _invoiceAppService.GetCreateVmAsync(JobId, BlId, null, cancellationToken);
        vm.JobId = JobId;
        vm.CompanyId = 1;
        vm.DefaultTaxPercent = 10;
        return PartialView("~/Views/Invoices/_InvoiceIssueDO.cshtml", vm);
    }

    [HttpGet]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> EditModal(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetEditVmAsync(id, cancellationToken);
        if (vm == null) return NotFound();
        return PartialView("~/Views/Invoices/_InvoiceEditModal.cshtml", vm);
    }

    [HttpPost]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> SaveHeader(InvoiceEditVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            if (vm.Id.HasValue)
            {
                var editVm = await _invoiceAppService.GetEditVmAsync(vm.Id.Value, cancellationToken);
                if (editVm != null)
                {
                    editVm.FinanceDirectionId = vm.FinanceDirectionId;
                    editVm.InvoiceTypeId = vm.InvoiceTypeId;
                    editVm.InvoiceStatusId = vm.InvoiceStatusId;
                    editVm.InvoiceNo = vm.InvoiceNo;
                    editVm.InvoiceDate = vm.InvoiceDate;
                    editVm.DueDate = vm.DueDate;
                    editVm.PartyId = vm.PartyId;
                    editVm.PartyName = vm.PartyName;
                    editVm.CurrencyId = vm.CurrencyId;
                    editVm.InvoicePNo = vm.InvoicePNo;
                    editVm.InvoicePDate = vm.InvoicePDate;
                    editVm.ExchangeRate = vm.ExchangeRate;
                    editVm.Remarks = vm.Remarks;
                    editVm.Notes = vm.Notes;
                    editVm.DefaultTaxPercent = vm.DefaultTaxPercent;
                    editVm.BLId = vm.BLId;
                    editVm.BookingId = vm.BookingId;
                    return PartialView("_InvoiceEditModal", editVm);
                }
            }
            var createVm = await _invoiceAppService.GetCreateVmAsync(vm.JobId, vm.BLId, vm.BookingId, cancellationToken);
            createVm.JobId = vm.JobId;
            createVm.FinanceDirectionId = vm.FinanceDirectionId;
            createVm.InvoiceTypeId = vm.FinanceDirectionId;
            createVm.InvoiceStatusId = vm.InvoiceStatusId;
            createVm.InvoiceNo = vm.InvoiceNo;
            createVm.InvoiceDate = vm.InvoiceDate;
            createVm.DueDate = vm.DueDate;
            createVm.PartyId = vm.PartyId;
            createVm.PartyName = vm.PartyName;
            createVm.CurrencyId = vm.CurrencyId;
            createVm.InvoicePNo = vm.InvoicePNo;
            createVm.InvoicePDate = vm.InvoicePDate;
            createVm.ExchangeRate = vm.ExchangeRate;
            createVm.Remarks = vm.Remarks;
            createVm.Notes = vm.Notes;
            createVm.DefaultTaxPercent = vm.DefaultTaxPercent;
            createVm.InvoiceSubjectId = vm.InvoiceSubjectId;
            createVm.BLId = vm.BLId;
            createVm.BookingId = vm.BookingId;
            return PartialView("_InvoiceEditModal", createVm);
        }


        var id = await _invoiceAppService.SaveHeaderAsync(vm, cancellationToken);

        if (vm.JobId == null || vm.JobId == Guid.Empty) Response.Headers["HX-Trigger"] = "invoice-list-refresh";

        var savedVm = await _invoiceAppService.GetEditVmAsync(id, cancellationToken);
        return PartialView("_InvoiceEditModal", savedVm);
    }


    [HttpPost]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> SaveDOInvoice(InvoiceEditVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            if (vm.Id.HasValue)
            {
                var editVm = await _invoiceAppService.GetEditVmAsync(vm.Id.Value, cancellationToken);
                if (editVm != null)
                {
                    editVm.PartyId = vm.PartyId;
                    editVm.PartyName = vm.PartyName;
                    editVm.Remarks = vm.Remarks;
                    editVm.DefaultTaxPercent = vm.DefaultTaxPercent;
                    editVm.BLId = vm.BLId;
                    editVm.JobId = vm.JobId;
                    return PartialView("_InvoiceIssueDO", editVm);
                }
            }
            var createVm = await _invoiceAppService.GetCreateVmAsync(vm.JobId, vm.BLId, vm.BookingId, cancellationToken);
            createVm.JobId = vm.JobId;
            createVm.PartyId = vm.PartyId;
            createVm.PartyName = vm.PartyName;
            createVm.Remarks = vm.Remarks;
            createVm.DefaultTaxPercent = vm.DefaultTaxPercent;
            createVm.BLId = vm.BLId;
            return PartialView("_InvoiceIssueDO", createVm);
        }

        if (await _invoiceAppService.ExistsSaleDoInvoiceForBLAsync(vm.BLId))
        {
            if (vm.Id.HasValue)
            {
                var editVm = await _invoiceAppService.GetEditVmAsync(vm.Id.Value, cancellationToken);
                if (editVm != null)
                {
                    editVm.PartyId = vm.PartyId;
                    editVm.PartyName = vm.PartyName;
                    editVm.Remarks = vm.Remarks;
                    editVm.DefaultTaxPercent = vm.DefaultTaxPercent;
                    editVm.BLId = vm.BLId;
                    editVm.JobId = vm.JobId;
                    ModelState.AddModelError("DoInvoiceExists", "DO Invoice Already Exists For This BL!!!!");
                    return PartialView("_InvoiceIssueDO", editVm);
                }
            }
            var createVm = await _invoiceAppService.GetCreateVmAsync(vm.JobId, vm.BLId, vm.BookingId, cancellationToken);
            createVm.JobId = vm.JobId;
            createVm.PartyId = vm.PartyId;
            createVm.PartyName = vm.PartyName;
            createVm.Remarks = vm.Remarks;
            createVm.DefaultTaxPercent = vm.DefaultTaxPercent;
            createVm.BLId = vm.BLId;
            ModelState.AddModelError("DoInvoiceExists", "DO Invoice Already Exists For This BL!!!!");
            return PartialView("_InvoiceIssueDO", createVm);
        }
        var id = await _invoiceAppService.SaveInvoiceDOAsync(vm, cancellationToken);

        //Response.Headers["HX-Trigger"] = "invoice-list-refresh";

        HxTrigger(new
        {
            refreshInvoicesGrid = true,
            toastSuccess = "Created Sussefully"
        });
        return NoContent();
        //var savedVm = await _invoiceAppService.GetEditVmAsync(id, cancellationToken);
        //return PartialView("_InvoiceEditModal", savedVm);
    }
    [HttpPost]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> Delete(Guid id, [FromForm] InvoiceListFilterVm filter, CancellationToken cancellationToken)
    {
        var allowStatus = await _invoiceAppService.InvoiceCanBeDeleted(id, cancellationToken);
        if (!allowStatus.canBeDeleted)
        {
            HxTrigger(new
            {
                toastFail = allowStatus.message
            });
            return NoContent();
        }
        await _invoiceAppService.SoftDeleteInvoiceAsync(id, cancellationToken);
        var vm = await _invoiceAppService.GetListPageAsync(filter, cancellationToken);

        HxTrigger(new
        {
            refreshInvoicesGrid = true,
            toastSuccess = "Invocie Deleted Sussefully"
        });


        return PartialView("_InvoiceList", vm);
    }

    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> ItemsGrid(Guid invoiceId, CancellationToken cancellationToken)
    {
        var items = await _invoiceAppService.GetItemsAsync(invoiceId, cancellationToken);
        return PartialView("_InvoiceItemsGrid", items);
    }

    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> ItemsGrid2(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoiceNo = await _invoiceAppService.GetInvoiceNoAsync(invoiceId);
        ViewBag.InvoiceNo = invoiceNo;
        var items = await _invoiceAppService.GetItemsAsync(invoiceId, cancellationToken);
        return PartialView("_InvoiceItemsGrid2", items);
    }


    [HttpGet]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> ItemForm(Guid invoiceId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetNewItemVmAsync(invoiceId, cancellationToken);

        var invoiceVm = await _invoiceAppService.GetEditVmAsync(invoiceId, cancellationToken);
        ViewData["FinanceDirectionId"] = invoiceVm?.FinanceDirectionId ?? 0;

        return PartialView("_InvoiceItemForm", vm);
    }

    [HttpGet]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> EditItemForm(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetEditItemVmAsync(id, cancellationToken);
        if (vm == null) return NotFound();

        var invoiceVm = await _invoiceAppService.GetEditVmAsync(vm.InvoiceId, cancellationToken);
        ViewData["FinanceDirectionId"] = invoiceVm?.FinanceDirectionId ?? 0;

        return PartialView("_InvoiceItemForm", vm);
    }

    [HttpPost]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> SaveItem(InvoiceItemEditVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var loadedVm = vm.Id.HasValue
                ? await _invoiceAppService.GetEditItemVmAsync(vm.Id.Value, cancellationToken)
                : await _invoiceAppService.GetNewItemVmAsync(vm.InvoiceId, cancellationToken);

            if (loadedVm == null)
                return NotFound();

            loadedVm.Id = vm.Id;
            loadedVm.InvoiceId = vm.InvoiceId;
            loadedVm.SequenceNumber = vm.SequenceNumber;
            loadedVm.ChargeTypeId = vm.ChargeTypeId;
            loadedVm.ChargeId = vm.ChargeId;
            loadedVm.Description = vm.Description;
            loadedVm.QTY = vm.QTY;
            loadedVm.UnitPrice = vm.UnitPrice;
            loadedVm.Discount = vm.Discount;
            loadedVm.TaxPercent = vm.TaxPercent;
            loadedVm.CurrencyId = await _invoiceAppService.GetInvoiceCurrencyIdAsync(vm.InvoiceId);
            loadedVm.ChargeTypeSupplementRemarks = vm.ChargeTypeSupplementRemarks;
            loadedVm.AmountBeforeDiscount = vm.QTY * vm.UnitPrice;
            loadedVm.AmountAfterDiscount = (vm.QTY * vm.UnitPrice) - vm.Discount;
            if (loadedVm.AmountAfterDiscount < 0) loadedVm.AmountAfterDiscount = 0;
            loadedVm.TaxAmount = loadedVm.AmountAfterDiscount * vm.TaxPercent / 100m;
            loadedVm.TotalAmount = loadedVm.AmountAfterDiscount + loadedVm.TaxAmount;

            var invoiceVm = await _invoiceAppService.GetEditVmAsync(vm.InvoiceId, cancellationToken);
            ViewData["FinanceDirectionId"] = invoiceVm?.FinanceDirectionId ?? 0;

            return PartialView("_InvoiceItemForm", loadedVm);
        }
        await _invoiceAppService.SaveItemAsync(vm, cancellationToken);

        var myVm = await _invoiceAppService.GetEditVmAsync(vm.InvoiceId, cancellationToken);

        if (myVm.JobId == null || myVm.JobId == Guid.Empty)
            Response.Headers["HX-Trigger"] = "invoice-items-refresh,invoice-totals-refresh,invoice-list-refresh";
        else
            Response.Headers["HX-Trigger"] = "invoice-items-refresh,invoice-totals-refresh";

        var resetVm = await _invoiceAppService.GetNewItemVmAsync(vm.InvoiceId, cancellationToken);
        return PartialView("_InvoiceItemForm", resetVm);
    }

    [HttpPost]
    [Authorize("Finance.Invoices.Edit")]
    public async Task<IActionResult> DeleteItem(Guid id, Guid invoiceId, CancellationToken cancellationToken)
    {
        await _invoiceAppService.SoftDeleteItemAsync(id, cancellationToken);
        Response.Headers["HX-Trigger"] = "invoice-items-refresh,invoice-totals-refresh,invoice-list-refresh";
        return NoContent();
    }
    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> Totals(Guid invoiceId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetTotalsAsync(invoiceId, cancellationToken);
        return PartialView("_InvoiceTotals", vm);
    }
    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public IActionResult PartyLookupModal(Guid? jobId)
    {
        ViewBag.JobId = jobId;
        return PartialView("_PartyLookupModal");
    }
    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> PartyLookupList([FromQuery] PartyLookupFilterVm filter, Guid? jobId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetPartyLookupAsync(filter, jobId, cancellationToken);
        ViewBag.Filter = filter;
        return PartialView("_PartyLookupGrid", vm);
    }
    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> ChargesByFilters(int? currencyId, int? chargeTypeId, int? financeDirectionId, string? selectedId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetChargesForItemAsync(currencyId, chargeTypeId, financeDirectionId, cancellationToken);
        ViewBag.SelectedId = selectedId;
        return PartialView("_ChargeOptions", vm);
    }


    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> PrintInvoice(Guid id)
    {
        var pdfBytes = await _reportService.GenerateInvoicePdfAsync(id);
        if (pdfBytes == null) return NotFound();
        //return File(pdfBytes, "application/pdf", $"Invoice_{DateTime.Now.Ticks}.pdf");
        return File(pdfBytes, "application/pdf");
    }

    [HttpGet]
    [Authorize("Finance.Invoices.View")]
    public async Task<IActionResult> PrintOfficialTaxInvoice(Guid Id)
    {
        // واکشی داده‌ها و ساخت DTO از طریق سرویس
        var invoiceDto = await _invoiceAppService.GetInvoicePrintDtoAsync(Id);
        if (invoiceDto == null)
            return NotFound();
        if (invoiceDto.FinanceDirectionId == 2) return NoContent();
        // واکشی لوگوی شرکت در صورت وجود
        var pdfBytes = await _invoiceAppService.GenerateOfficialTaxInvoicePdfAsync(invoiceDto, default);
        //return File(pdfBytes, "application/pdf", $"Invoice-{invoiceDto.InvoiceNo}.pdf");
        return File(pdfBytes, "application/pdf");
    }
}