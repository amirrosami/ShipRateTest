using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.ViewModels;
using System.Security.Claims;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class AccountCodeController : Controller
    {
        private readonly IAccountCodeService _accountCodeService;

        public AccountCodeController(IAccountCodeService accountCodeService)
        {
            _accountCodeService = accountCodeService;
        }
        [Authorize("Finance.AccountCodes.View")]
        public IActionResult Index()
        {
            var model = new AccountCodeSearchVm
            {
                Page = 1,
                PageSize = 10
            };

            return View(model);
        }

        [HttpGet]
        [Authorize("Finance.AccountCodes.View")]
        public async Task<IActionResult> List(AccountCodeSearchVm input)
        {
            if (input.Page <= 0) input.Page = 1;
            if (input.PageSize <= 0) input.PageSize = 10;

            var result = await _accountCodeService.GetPagedAsync(input);
            ViewBag.SearchVm = input;
            return PartialView("_List", result);
        }

        [HttpGet]
        [Authorize("Finance.AccountCodes.Edit")]
        public async Task<IActionResult> Create()
        {
            var vm = await _accountCodeService.GetCreateVmAsync();
            return PartialView("_CreateEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.AccountCodes.Edit")]
        public async Task<IActionResult> Create(AccountCodeCreateEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await _accountCodeService.FillLookupsAsync(vm);
                Response.Headers["HX-Retarget"] = "#accountCodeModalBody";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateEdit", vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountCodeService.CreateAsync(vm, userId);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                vm = await _accountCodeService.FillLookupsAsync(vm);
                Response.Headers["HX-Retarget"] = "#accountCodeModalBody";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateEdit", vm);
            }

            Response.Headers["HX-Trigger"] = "accountCodeChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        [Authorize("Finance.AccountCodes.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await _accountCodeService.GetByIdForEditAsync(id);
            if (vm == null)
                return NotFound();

            return PartialView("_CreateEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.AccountCodes.Edit")]
        public async Task<IActionResult> Edit(AccountCodeCreateEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await _accountCodeService.FillLookupsAsync(vm);
                Response.Headers["HX-Retarget"] = "#accountCodeModalBody";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateEdit", vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountCodeService.UpdateAsync(vm, userId);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                vm = await _accountCodeService.FillLookupsAsync(vm);
                Response.Headers["HX-Retarget"] = "#accountCodeModalBody";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateEdit", vm);
            }

            Response.Headers["HX-Trigger"] = "accountCodeChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        [Authorize("Finance.AccountCodes.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vm = await _accountCodeService.GetByIdForEditAsync(id);
            if (vm == null)
                return NotFound();

            return PartialView("_Delete", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.AccountCodes.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accountCodeService.DeleteAsync(id, userId);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            Response.Headers["HX-Trigger"] = "accountCodeChanged";
            return Content(string.Empty);
        }
    }
}
