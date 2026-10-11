using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class TreasuryDocumentsController : Controller
    {
        private readonly ITreasuryDocumentService _service;
        private readonly ICurrentUserService _currentUserService;


        public TreasuryDocumentsController(ITreasuryDocumentService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize("Finance.TreasuryDocuments.View")]
        public async Task<IActionResult> Index([FromQuery] TreasuryDocumentSearchDto search)
        {
            search.PageNumber = search.PageNumber <= 0 ? 1 : search.PageNumber;
            search.PageSize = search.PageSize is 10 or 20 or 50 ? search.PageSize : 10;

            var result = await _service.SearchAsync(search);

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_List", result);

            var vm = new TreasuryDocumentIndexVm
            {
                Search = search,
                Result = result,
                Statuses = await _service.GetTreasuryDocumentStatusSelectListAsync()
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize("Finance.TreasuryDocuments.Edit")]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildFormVmAsync(await _service.GetCreateModelAsync());
            return PartialView("_CreateEditModal", vm);
        }

        [HttpPost]
        [Authorize("Finance.TreasuryDocuments.Edit")]
        public async Task<IActionResult> Create(TreasuryDocumentSaveDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return await ReturnFormWithErrors(input);

                await _service.CreateAsync(input, GetCurrentUserId());

                Response.Headers["HX-Trigger"] = "treasuryDocumentChanged";
                return Content("");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await ReturnFormWithErrors(input);
            }
        }

        [HttpGet]
        [Authorize("Finance.TreasuryDocuments.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var form = await _service.GetEditModelAsync(id);

            if (form == null)
                return NotFound();

            var vm = await BuildFormVmAsync(form);
            return PartialView("_CreateEditModal", vm);
        }

        [HttpPost]
        [Authorize("Finance.TreasuryDocuments.Edit")]
        public async Task<IActionResult> Edit(TreasuryDocumentSaveDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return await ReturnFormWithErrors(input);

                await _service.UpdateAsync(input, GetCurrentUserId());

                Response.Headers["HX-Trigger"] = "treasuryDocumentChanged";
                return Content("");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await ReturnFormWithErrors(input);
            }
        }

        [HttpGet]
        [Authorize("Finance.TreasuryDocuments.View")]
        public async Task<IActionResult> PersonLookup(string partySearch)
        {
            var items = await _service.SearchPersonsAsync(partySearch ?? "");
            return PartialView("_PersonLookup", items);
        }

        [HttpGet]
        [Authorize("Finance.TreasuryDocuments.View")]
        public async Task<IActionResult> AvailablePayments(Guid partyId, Guid? treasuryDocumentId)
        {
            if (partyId == Guid.Empty)
                return PartialView("_PaymentDocumentsSelector", new List<TreasuryDocumentPaymentSelectorDto>());

            var items = await _service.GetAvailablePaymentDocumentsByPartyAsync(partyId, treasuryDocumentId);
            return PartialView("_PaymentDocumentsSelector", items);
        }

        private async Task<TreasuryDocumentFormVm> BuildFormVmAsync(TreasuryDocumentFormDto form)
        {
            return new TreasuryDocumentFormVm
            {
                Form = form,
                Companies = await _service.GetCompanySelectListAsync(),
                Currencies = await _service.GetCurrencySelectListAsync(),
                Types = await _service.GetTreasuryDocumentTypeSelectListAsync(),
                Statuses = await _service.GetTreasuryDocumentStatusSelectListAsync()
            };
        }

        private async Task<IActionResult> ReturnFormWithErrors(TreasuryDocumentSaveDto input)
        {
            var form = new TreasuryDocumentFormDto
            {
                Id = input.Id,
                CompanyId = input.CompanyId,
                DocumentNo = input.DocumentNo,
                TreasuryDocumentTypeId = input.TreasuryDocumentTypeId,
                DocumentDate = input.DocumentDate == default ? DateTime.Today : input.DocumentDate,
                PartyId = input.PartyId,
                CurrencyId = input.CurrencyId,
                Description = input.Description,
                TreasuryDocumentStatusId = input.TreasuryDocumentStatusId
            };

            if (input.PartyId.HasValue)
            {
                var payments = await _service.GetAvailablePaymentDocumentsByPartyAsync(input.PartyId.Value, input.Id);

                foreach (var payment in payments)
                    payment.IsSelected = input.SelectedPaymentDocumentIds.Contains(payment.PaymentDocumentId);

                form.PaymentItems = payments;
            }

            var vm = await BuildFormVmAsync(form);
            return PartialView("_CreateEditModal", vm);
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.TreasuryDocuments.Edit")]
        public async Task<IActionResult> Delete(Guid id, TreasuryDocumentSearchDto search)
        {
            var result = await _service.DeleteAsync(id);

            Response.Headers["HX-Trigger"] = System.Text.Json.JsonSerializer.Serialize(new
            {
                showMessage = result.Message
            });

            var model = await _service.SearchAsync(search);
            return PartialView("_List", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.TreasuryDocuments.Confirm")]
        public async Task<IActionResult> Confirm(Guid id, TreasuryDocumentSearchDto search)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                await _service.ConfirmAsync(id, currentUserId);

                var vm = new TreasuryDocumentIndexVm
                {
                    Search = search,
                    Result = await _service.SearchAsync(search),
                    Statuses = await _service.GetTreasuryDocumentStatusSelectListAsync()
                };

                return PartialView("_List", vm);
            }
            catch (Exception ex)
            {
                Response.Headers["HX-Retarget"] = "#listMessage";
                return Content($"<div class='alert alert-danger'>{ex.Message}</div>");
            }
        }











    }
}
