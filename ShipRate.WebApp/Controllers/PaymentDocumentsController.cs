using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class PaymentDocumentsController : Controller
    {
        private readonly IPaymentDocumentService _paymentDocumentService;
        public PaymentDocumentsController(IPaymentDocumentService paymentDocumentService)
        {
            _paymentDocumentService = paymentDocumentService;
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }
        [HttpGet]
        [Authorize("Finance.PaymentDocuments.View")]
        public async Task<IActionResult> Index([FromQuery] PaymentDocumentSearchVm search, CancellationToken cancellationToken)
        {
            search ??= new PaymentDocumentSearchVm();
            await PopulateSearchDropdownsAsync(search);
            var result = await _paymentDocumentService.GetPagedAsync(search);
            ViewBag.Search = search;
            return View(result);
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.View")]
        public async Task<IActionResult> List([FromQuery] PaymentDocumentSearchVm search, CancellationToken cancellationToken)
        {
            search ??= new PaymentDocumentSearchVm();

            await PopulateSearchDropdownsAsync(search);
            var result = await _paymentDocumentService.GetPagedAsync(search);
            ViewBag.Search = search;
            return PartialView("_List", result);
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.Edit")]
        public async Task<IActionResult> Create(Guid? partyId, int? companyId, int? currencyId, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetCreateModelAsync();
            if (partyId != null && partyId.HasValue && partyId != Guid.Empty) model.PersonId = partyId.Value;
            if (currencyId != null && currencyId.HasValue) model.CurrencyId = currencyId.Value;
            if (companyId != null && companyId.HasValue) model.CompanyId = companyId.Value;
            return PartialView("_CreateEditModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.PaymentDocuments.Edit")]
        public async Task<IActionResult> Create(PaymentDocumentCreateEditVm model, CancellationToken cancellationToken)
        {
            //if (model.PaymentMethodId == 3)
            //{
            //    if (model.Cheque == null)
            //    {
            //        ModelState.AddModelError("Cheque", "Cheque info should be entered");
            //    }
            //    if (!model.Cheque.IBAN.IsNullOrEmpty())
            //    {
            //        if (!ShebaValidator.IsValidSheba(model.Cheque.IBAN))
            //        {
            //            ModelState.AddModelError("Sheba", "Sheba (IBAN) is  not valid!!!!");
            //        }
            //    }
            //}

            if (!ModelState.IsValid)
            {
                var createModel = await _paymentDocumentService.GetCreateModelAsync();

                model.Companies = createModel.Companies;
                model.Currencies = createModel.Currencies;
                model.FinanceDirections = createModel.FinanceDirections;
                model.PaymentMethods = createModel.PaymentMethods;
                model.Banks = createModel.Banks;
                //model.BankAccounts = await _paymentDocumentService.GetBankAccountsByBankIdAsync(
                //    model.Cheque?.BankId
                //    );
                model.ChequeTypes = createModel.ChequeTypes;
                model.ChequePurposes = createModel.ChequePurposes;

                Response.Headers["HX-Retarget"] = "#paymentDocumentModalContent";
                Response.Headers["HX-Reswap"] = "innerHTML";

                return PartialView("_CreateEditModal", model);

            }





            await _paymentDocumentService.CreateAsync(model);
            //Response.Headers["HX-Trigger"] = "paymentDocumentChanged";
            //return PartialView("_SuccessCloseModal");
            HxTrigger(new
            {
                paymentDocumentChanged = true,
                paymentDocumentSaved = true,
                toastSuccess = "Payment Saved Successfully"
            });
            return NoContent();
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.Edit")]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetEditModelAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_CreateEditModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.PaymentDocuments.Edit")]
        public async Task<IActionResult> Edit(Guid id, PaymentDocumentCreateEditVm model, CancellationToken cancellationToken)
        {

            if (model.PaymentMethodId == 3)
            {
                if (model.Cheque == null)
                {
                    ModelState.AddModelError("Cheque", "Cheque info should be entered");
                }
                if (!model.Cheque.IBAN.IsNullOrEmpty())
                {
                    if (!ShebaValidator.IsValidSheba(model.Cheque.IBAN))
                    {
                        ModelState.AddModelError("Sheba", "Sheba (IBAN) is  not valid!!!!");
                    }
                }
            }




            if (!ModelState.IsValid)
            {
                var editModel = await _paymentDocumentService.GetEditModelAsync(id);
                if (editModel == null)
                    return NotFound();

                model.Companies = editModel.Companies;
                model.Currencies = editModel.Currencies;
                model.FinanceDirections = editModel.FinanceDirections;
                model.PaymentMethods = editModel.PaymentMethods;
                model.Banks = editModel.Banks;
                model.BankAccounts = await _paymentDocumentService.GetBankAccountsByBankIdAsync(
                    model.Cheque?.BankId
                    );
                model.ChequeTypes = editModel.ChequeTypes;
                model.ChequePurposes = editModel.ChequePurposes;

                Response.Headers["HX-Retarget"] = "#payment-document-modal .modal-content";
                Response.Headers["HX-Reswap"] = "innerHTML";

                return PartialView("_CreateEditModal", model);
            }

            var updated = await _paymentDocumentService.UpdateAsync(model);
            if (!updated)
                return NotFound();
            //Response.Headers["HX-Trigger"] = "paymentDocumentChanged";
            HxTrigger(
                new
                {
                    paymentDocumentChanged = true,
                    toastSuccess = "Payment Saved Successfully"
                }
                );
            return PartialView("_SuccessCloseModal");
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.View")]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetDetailsAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_DetailsModal", model);
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.Edit")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetDetailsAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.PaymentDocuments.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            if (!await _paymentDocumentService.CanBeDeletedPaymentDocumentAsync(id))
            {
                HxTrigger(new
                {
                    toastFail = "Used Payment can not be deleted!!!!"
                });
                return NoContent();
            }
            var deleted = await _paymentDocumentService.SoftDeleteAsync(id);
            if (!deleted)
                return NotFound();
            Response.Headers["HX-Trigger"] = "paymentDocumentChanged";
            return PartialView("_SuccessCloseModal");
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.View")]
        public async Task<IActionResult> SearchPersons(string? term, CancellationToken cancellationToken)
        {
            var result = await _paymentDocumentService.SearchPersonsAsync(term);
            return PartialView("_PersonSearchResults", result);
        }

        [HttpGet]
        [Authorize("Finance.PaymentDocuments.View")]
        public async Task<IActionResult> BankAccounts(Guid? bankId, Guid? selectedId, CancellationToken cancellationToken)
        {
            var items = await _paymentDocumentService.GetBankAccountsByBankIdAsync(bankId);
            return PartialView("_BankAccountOptions", items);
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailablePayments(Guid? partyId, int? currencyId, int? companyId)
        {
            if (!partyId.HasValue)
                return PartialView("_AvailablePaymentDocsPartial", new List<AvailablePaymentDocDto>());
            if (!companyId.HasValue)
                return PartialView("_AvailablePaymentDocsPartial", new List<AvailablePaymentDocDto>());



            var paymentDocs = await _paymentDocumentService.GetAvailablePaymentDocumentsAsync(partyId.Value, currencyId, companyId.Value);
            return PartialView("_AvailablePaymentDocsPartial", paymentDocs);
        }
        private async Task PopulateSearchDropdownsAsync(PaymentDocumentSearchVm search)
        {
            var baseLookups = await _paymentDocumentService.GetCreateModelAsync();
            search.Companies = baseLookups.Companies ?? new List<SelectListItem>();
            search.Currencies = baseLookups.Currencies ?? new List<SelectListItem>();
            search.FinanceDirections = baseLookups.FinanceDirections ?? new List<SelectListItem>();
            search.PaymentMethods = baseLookups.PaymentMethods ?? new List<SelectListItem>();
            search.ChequeTypes = baseLookups.ChequeTypes ?? new List<SelectListItem>();
            search.ChequePurposes = baseLookups.ChequePurposes ?? new List<SelectListItem>();
        }

        [HttpGet]
        public async Task<IActionResult> Usages(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetUsagePanelAsync(id, cancellationToken);
            if (model is null)
                return NotFound();
            return PartialView("_PaymentDocumentUsages", model);
        }
    }
}

