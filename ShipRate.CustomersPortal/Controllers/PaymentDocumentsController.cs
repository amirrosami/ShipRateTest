using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;

namespace ShipRate.CustomersPortal.Controllers
{
    public class PaymentDocumentsController : Controller
    {
        private readonly IPaymentDocumentService _paymentDocumentService;
        private readonly ICurrentUserService currentUserService;
        public PaymentDocumentsController(IPaymentDocumentService paymentDocumentService, ICurrentUserService currentUserService)
        {
            _paymentDocumentService = paymentDocumentService;
            this.currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] PaymentDocumentSearchVm search, CancellationToken cancellationToken)
        {
            search ??= new PaymentDocumentSearchVm();

            var personId = await currentUserService.GetPersonIdAsync();
            search.PersonId = personId;
            search.FinanceDirectionId = 1;
            await PopulateSearchDropdownsAsync(search);
            var result = await _paymentDocumentService.GetPagedAsync(search);
            ViewBag.Search = search;
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PaymentDocumentSearchVm search, CancellationToken cancellationToken)
        {
            search ??= new PaymentDocumentSearchVm();
            var personId = await currentUserService.GetPersonIdAsync();
            search.PersonId = personId;
            search.FinanceDirectionId = 1;
            await PopulateSearchDropdownsAsync(search);
            var result = await _paymentDocumentService.GetPagedAsync(search);
            ViewBag.Search = search;
            return PartialView("_List", result);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetCreateModelAsync();
            return PartialView("_CreateEditModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentDocumentCreateEditVm model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var createModel = await _paymentDocumentService.GetCreateModelAsync();

                model.Companies = createModel.Companies;
                model.Currencies = createModel.Currencies;
                model.FinanceDirections = createModel.FinanceDirections;
                model.PaymentMethods = createModel.PaymentMethods;
                model.Banks = createModel.Banks;
                model.BankAccounts = await _paymentDocumentService.GetBankAccountsByBankIdAsync(
                    model.Cheque?.BankId
                    );
                model.ChequeTypes = createModel.ChequeTypes;
                model.ChequePurposes = createModel.ChequePurposes;

                Response.Headers["HX-Retarget"] = "#payment-document-modal .modal-content";
                Response.Headers["HX-Reswap"] = "innerHTML";

                return PartialView("_CreateEditModal", model);
            }

            await _paymentDocumentService.CreateAsync(model);

            Response.Headers["HX-Trigger"] = "paymentDocumentChanged";
            return PartialView("_SuccessCloseModal");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetEditModelAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_CreateEditModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PaymentDocumentCreateEditVm model, CancellationToken cancellationToken)
        {
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

            Response.Headers["HX-Trigger"] = "paymentDocumentChanged";
            return PartialView("_SuccessCloseModal");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetDetailsAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_DetailsModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var model = await _paymentDocumentService.GetDetailsAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_DeleteModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
        {
            var deleted = await _paymentDocumentService.SoftDeleteAsync(id);
            if (!deleted)
                return NotFound();

            Response.Headers["HX-Trigger"] = "paymentDocumentChanged";
            return PartialView("_SuccessCloseModal");
        }

        [HttpGet]
        public async Task<IActionResult> SearchPersons(string? term, CancellationToken cancellationToken)
        {
            var result = await _paymentDocumentService.SearchPersonsAsync(term);
            return PartialView("_PersonSearchResults", result);
        }

        [HttpGet]
        public async Task<IActionResult> BankAccounts(Guid? bankId, Guid? selectedId, CancellationToken cancellationToken)
        {
            var items = await _paymentDocumentService.GetBankAccountsByBankIdAsync(bankId);
            return PartialView("_BankAccountOptions", items);
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
    }
}
