using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.Finance;
using System.Security.Claims;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public sealed class PettyCashController : Controller
    {
        private readonly IPettyCashService _service;

        public PettyCashController(IPettyCashService service)
        {
            _service = service;
        }

        [Authorize("PettyCash.PettyCashes.View")]
        public async Task<IActionResult> Index(PettyCashIndexSearchDto search, CancellationToken cancellationToken)
        {
            var model = await _service.GetPettyCashesAsync(search, cancellationToken);
            ViewBag.Search = search;
            ViewBag.Lookups = await _service.GetLookupsAsync(search.CompanyId, cancellationToken);
            return View(model);
        }
        [Authorize("PettyCash.PettyCashes.View")]
        public async Task<IActionResult> Grid(PettyCashIndexSearchDto search, CancellationToken cancellationToken)
        {
            var model = await _service.GetPettyCashesAsync(search, cancellationToken);
            ViewBag.Search = search;
            return PartialView("_PettyCashGrid", model);
        }

        [HttpGet]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> CreatePettyCash(CancellationToken cancellationToken)
        {
            ViewBag.Lookups = await _service.GetLookupsAsync(null, cancellationToken);
            var model = await _service.GetPettyCashForCreateAsync(cancellationToken);
            return PartialView("_PettyCashForm", model);
        }

        [HttpGet]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> EditPettyCash(Guid id, CancellationToken cancellationToken)
        {
            var model = await _service.GetPettyCashForEditAsync(id, cancellationToken);
            ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
            return PartialView("_PettyCashForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> SavePettyCash(PettyCashUpsertDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
                Response.StatusCode = 400;
                return PartialView("_PettyCashForm", model);
            }

            await _service.SavePettyCashAsync(model, GetCurrentUserId(), cancellationToken);
            Response.Headers["HX-Trigger"] = "pettyCashChanged";
            return Content("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> DeletePettyCash(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeletePettyCashAsync(id, GetCurrentUserId(), cancellationToken);
            Response.Headers["HX-Trigger"] = "pettyCashChanged";
            return Content("");
        }


        [Authorize("PettyCash.PettyCashes.View")]
        public async Task<IActionResult> Expenses(Guid pettyCashId, CancellationToken cancellationToken)
        {
            ViewBag.PettyCashId = pettyCashId;
            var model = await _service.GetExpensesAsync(pettyCashId, cancellationToken);
            return PartialView("_ExpensesPanel", model);
        }

        [HttpGet]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> CreateExpense(Guid pettyCashId, CancellationToken cancellationToken)
        {
            var model = await _service.GetExpenseForCreateAsync(pettyCashId, cancellationToken);
            ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
            return PartialView("_ExpenseForm", model);
        }

        [HttpGet]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> EditExpense(Guid id, CancellationToken cancellationToken)
        {
            var model = await _service.GetExpenseForEditAsync(id, cancellationToken);
            ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
            return PartialView("_ExpenseForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> SaveExpense(PettyCashExpenseUpsertDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
                Response.StatusCode = 400;
                return PartialView("_ExpenseForm", model);
            }

            await _service.SaveExpenseAsync(model, GetCurrentUserId(), cancellationToken);
            Response.Headers["HX-Trigger"] = "expenseChanged,pettyCashChanged";
            return Content("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> DeleteExpense(Guid id, Guid pettyCashId, CancellationToken cancellationToken)
        {
            await _service.DeleteExpenseAsync(id, GetCurrentUserId(), cancellationToken);
            Response.Headers["HX-Trigger"] = "expenseChanged,pettyCashChanged";
            return Content("");
        }

        [Authorize("PettyCash.PettyCashes.View")]
        public async Task<IActionResult> Recharges(Guid pettyCashId, CancellationToken cancellationToken)
        {
            ViewBag.PettyCashId = pettyCashId;
            var model = await _service.GetRechargesAsync(pettyCashId, cancellationToken);
            return PartialView("_RechargesPanel", model);
        }

        [HttpGet]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> CreateRecharge(Guid pettyCashId, CancellationToken cancellationToken)
        {
            var model = await _service.GetRechargeForCreateAsync(pettyCashId, cancellationToken);
            ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
            return PartialView("_RechargeForm", model);
        }

        [HttpGet]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> EditRecharge(Guid id, CancellationToken cancellationToken)
        {
            var model = await _service.GetRechargeForEditAsync(id, cancellationToken);
            ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
            return PartialView("_RechargeForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> SaveRecharge(PettyCashRechargeUpsertDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Lookups = await _service.GetLookupsAsync(model.CompanyId, cancellationToken);
                Response.StatusCode = 400;
                return PartialView("_RechargeForm", model);
            }

            await _service.SaveRechargeAsync(model, GetCurrentUserId(), cancellationToken);
            Response.Headers["HX-Trigger"] = "rechargeChanged,pettyCashChanged";
            return Content("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> DeleteRecharge(Guid id, Guid pettyCashId, CancellationToken cancellationToken)
        {
            await _service.DeleteRechargeAsync(id, GetCurrentUserId(), cancellationToken);
            Response.Headers["HX-Trigger"] = "rechargeChanged,pettyCashChanged";
            return Content("");
        }

        private Guid GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.ApproveRejectExpense")]
        public async Task<IActionResult> ApproveExpense(Guid id, Guid pettyCashId)
        {
            await _service.ApproveExpenseAsync(id);

            Response.Headers["HX-Trigger"] = "expenseChanged";
            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.ApproveRejectExpense")]
        public async Task<IActionResult> RejectExpense(Guid id, Guid pettyCashId)
        {
            await _service.RejectExpenseAsync(id);

            Response.Headers["HX-Trigger"] = "expenseChanged";
            return NoContent();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.ApproveRejectRecharge")]
        public async Task<IActionResult> ApproveRecharge(Guid id, Guid pettyCashId)
        {
            await _service.ApproveRechargeAsync(id);

            Response.Headers["HX-Trigger"] = "rechargeChanged";
            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.ApproveRejectRecharge")]
        public async Task<IActionResult> RejectRecharge(Guid id, Guid pettyCashId)
        {
            await _service.RejectRechargeAsync(id);

            Response.Headers["HX-Trigger"] = "rechargeChanged";
            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("PettyCash.PettyCashes.Edit")]
        public async Task<IActionResult> ChargeRecharge(Guid id, Guid pettyCashId)
        {
            await _service.ChargeRechargeAsync(id);

            Response.Headers["HX-Trigger"] = "rechargeChanged";
            return NoContent();
        }
    }
}