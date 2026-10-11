using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Services;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebAppCustomers.Controllers;

[AutoValidateAntiforgeryToken]
public class InvoicesController : ApplicationController
{
    private readonly IInvoiceAppService _invoiceAppService;
    private readonly PaymentService paymentService;

    public InvoicesController(IInvoiceAppService invoiceAppService, PaymentService paymentService)
    {
        this.paymentService = paymentService;
        _invoiceAppService = invoiceAppService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetListPageAsync(new InvoiceListFilterVm(), cancellationToken);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] InvoiceListFilterVm filter, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetListPageAsync(filter, cancellationToken);
        return PartialView("_InvoiceList", vm);
    }

    //[HttpGet]
    //public async Task<IActionResult> CreateModal(Guid? JobId, CancellationToken cancellationToken)
    //{
    //    var vm = await _invoiceAppService.GetCreateVmAsync(JobId, cancellationToken);
    //    vm.JobId = JobId;
    //    return PartialView("~/Views/Invoices/_InvoiceEditModal.cshtml", vm);
    //}

    [HttpGet]
    public async Task<IActionResult> EditModal(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetEditVmAsync(id, cancellationToken);
        if (vm == null) return NotFound();
        return PartialView("~/Views/Invoices/_InvoiceEditModal.cshtml", vm);
    }

    [HttpPost]
    //public async Task<IActionResult> SaveHeader(InvoiceEditVm vm, CancellationToken cancellationToken)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        if (vm.Id.HasValue)
    //        {
    //            var editVm = await _invoiceAppService.GetEditVmAsync(vm.Id.Value, cancellationToken);
    //            if (editVm != null)
    //            {
    //                editVm.FinanceDirectionId = vm.FinanceDirectionId;
    //                editVm.InvoiceTypeId = vm.InvoiceTypeId;
    //                editVm.InvoiceStatusId = vm.InvoiceStatusId;
    //                editVm.InvoiceNo = vm.InvoiceNo;
    //                editVm.InvoiceDate = vm.InvoiceDate;
    //                editVm.DueDate = vm.DueDate;
    //                editVm.PartyId = vm.PartyId;
    //                editVm.PartyName = vm.PartyName;
    //                editVm.CurrencyId = vm.CurrencyId;
    //                editVm.InvoicePNo = vm.InvoicePNo;
    //                editVm.InvoicePDate = vm.InvoicePDate;
    //                editVm.ExchangeRate = vm.ExchangeRate;
    //                editVm.Remarks = vm.Remarks;
    //                editVm.Notes = vm.Notes;
    //                return PartialView("_InvoiceEditModal", editVm);
    //            }
    //        }

    //        var createVm = await _invoiceAppService.GetCreateVmAsync(vm.JobId, cancellationToken);
    //        createVm.FinanceDirectionId = vm.FinanceDirectionId;
    //        createVm.InvoiceTypeId = vm.InvoiceTypeId;
    //        createVm.InvoiceStatusId = vm.InvoiceStatusId;
    //        createVm.InvoiceNo = vm.InvoiceNo;
    //        createVm.InvoiceDate = vm.InvoiceDate;
    //        createVm.DueDate = vm.DueDate;
    //        createVm.PartyId = vm.PartyId;
    //        createVm.PartyName = vm.PartyName;
    //        createVm.CurrencyId = vm.CurrencyId;
    //        createVm.InvoicePNo = vm.InvoicePNo;
    //        createVm.InvoicePDate = vm.InvoicePDate;
    //        createVm.ExchangeRate = vm.ExchangeRate;
    //        createVm.Remarks = vm.Remarks;
    //        createVm.Notes = vm.Notes;
    //        return PartialView("_InvoiceEditModal", createVm);
    //    }

    //    var id = await _invoiceAppService.SaveHeaderAsync(vm, cancellationToken);

    //    Response.Headers["HX-Trigger"] = "invoice-list-refresh";

    //    var savedVm = await _invoiceAppService.GetEditVmAsync(id, cancellationToken);
    //    return PartialView("_InvoiceEditModal", savedVm);
    //}

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id, [FromForm] InvoiceListFilterVm filter, CancellationToken cancellationToken)
    {
        await _invoiceAppService.SoftDeleteInvoiceAsync(id, cancellationToken);
        var vm = await _invoiceAppService.GetListPageAsync(filter, cancellationToken);
        return PartialView("_InvoiceList", vm);
    }

    [HttpGet]
    public async Task<IActionResult> ItemsGrid(Guid invoiceId, CancellationToken cancellationToken)
    {
        var items = await _invoiceAppService.GetItemsAsync(invoiceId, cancellationToken);
        return PartialView("_InvoiceItemsGrid", items);
    }

    [HttpGet]
    public async Task<IActionResult> ItemsGrid2(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoiceNo = await _invoiceAppService.GetInvoiceNoAsync(invoiceId);
        ViewBag.InvoiceNo = invoiceNo;
        var items = await _invoiceAppService.GetItemsAsync(invoiceId, cancellationToken);
        return PartialView("_InvoiceItemsGrid2", items);
    }


    [HttpGet]
    public async Task<IActionResult> ItemForm(Guid invoiceId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetNewItemVmAsync(invoiceId, cancellationToken);

        var invoiceVm = await _invoiceAppService.GetEditVmAsync(invoiceId, cancellationToken);
        ViewData["FinanceDirectionId"] = invoiceVm?.FinanceDirectionId ?? 0;

        return PartialView("_InvoiceItemForm", vm);
    }

    [HttpGet]
    public async Task<IActionResult> EditItemForm(Guid id, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetEditItemVmAsync(id, cancellationToken);
        if (vm == null) return NotFound();

        var invoiceVm = await _invoiceAppService.GetEditVmAsync(vm.InvoiceId, cancellationToken);
        ViewData["FinanceDirectionId"] = invoiceVm?.FinanceDirectionId ?? 0;

        return PartialView("_InvoiceItemForm", vm);
    }

    [HttpPost]
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
            loadedVm.CurrencyId = vm.CurrencyId;
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

        Response.Headers["HX-Trigger"] = "invoice-items-refresh,invoice-totals-refresh,invoice-list-refresh";

        var resetVm = await _invoiceAppService.GetNewItemVmAsync(vm.InvoiceId, cancellationToken);
        return PartialView("_InvoiceItemForm", resetVm);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem(Guid id, Guid invoiceId, CancellationToken cancellationToken)
    {
        await _invoiceAppService.SoftDeleteItemAsync(id, cancellationToken);
        Response.Headers["HX-Trigger"] = "invoice-items-refresh,invoice-totals-refresh,invoice-list-refresh";
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> Totals(Guid invoiceId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetTotalsAsync(invoiceId, cancellationToken);
        return PartialView("_InvoiceTotals", vm);
    }

    [HttpGet]
    public IActionResult PartyLookupModal()
    {
        return PartialView("_PartyLookupModal");
    }

    [HttpGet]
    public async Task<IActionResult> PartyLookupList([FromQuery] PartyLookupFilterVm filter, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetPartyLookupAsync(filter, null, cancellationToken);
        ViewBag.Filter = filter;
        return PartialView("_PartyLookupGrid", vm);
    }

    [HttpGet]
    public async Task<IActionResult> ChargesByFilters(int? currencyId, int? chargeTypeId, int? financeDirectionId, string? selectedId, CancellationToken cancellationToken)
    {
        var vm = await _invoiceAppService.GetChargesForItemAsync(currencyId, chargeTypeId, financeDirectionId, cancellationToken);
        ViewBag.SelectedId = selectedId;
        return PartialView("_ChargeOptions", vm);
    }


    [HttpPost]
    public async Task<IActionResult> LoadPayModal(List<Guid> selectedInvoiceIds)
    {
        var res = await _invoiceAppService.GetNewOnlinePayment(selectedInvoiceIds);
        if (!res.Succeeded)
        {
            //HxTrigger(new { toastError = string.Join("-",res.Errors) });
            return NotFound(string.Join("-", res.Errors));
        }

        var onlineGateways = await paymentService.GetAvailableIPGTerminals();
        if (!onlineGateways.Succeeded)
        {
            //HxTrigger(new { toastError = string.Join("-", res.Errors) });
            return NotFound(string.Join("-", res.Errors));
        }

        ViewBag.OnlineGateways = onlineGateways.Data;
        return PartialView("_OnlinePaymentForm", res.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(CreateOnlinePaymentVm createVm)
    {
        var result = await paymentService.CreateOnlinePayment(createVm);
        if (!result.Succeeded)
        {
            AddModelStateErrors(result.Errors);
            return Content("");
        }

        HxTrigger(new { closeAppModal = true });

        // redirect to bank . 
        Response.Headers["HX-Redirect"] = result.Data.RedirectUrl;
        return Content("");
    }
}

