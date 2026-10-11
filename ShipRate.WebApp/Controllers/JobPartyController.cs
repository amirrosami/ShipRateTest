using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class JobPartyController : Controller
    {
        private readonly IJobPartyService _service;
        private readonly IJobService _jobService;
        private readonly ICurrentUserService _currentUserService;

        public JobPartyController(IJobPartyService service, IJobService jobService, ICurrentUserService currentUserService)
        {
            _service = service;
            _jobService = jobService;
            _currentUserService = currentUserService;
        }

        public IActionResult Panel(Guid jobId)
        {
            var vm = new JobPartyFilterVm
            {
                JobId = jobId
            };

            return PartialView("_JobPartyPanel", vm);
        }

        public async Task<IActionResult> List(JobPartyFilterVm filter)
        {
            var result = await _service.GetListAsync(filter);
            ViewBag.JobId = filter.JobId;
            return PartialView("_JobPartyList", result);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid jobId)
        {
            var vm = await _service.BuildCreateVmAsync(jobId);
            return PartialView("_Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobPartyFormVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.ContactEmail))
            {
                vm.ContactEmail = null;
                ModelState.Remove(nameof(vm.ContactEmail));
            }

            if (!ModelState.IsValid)
            {
                await _service.FillLookupsAsync(vm);
                return PartialView("_Create", vm);
            }

            var result = await _service.CreateAsync(vm, GetCurrentUserId());

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                await _service.FillLookupsAsync(vm);
                return PartialView("_Create", vm);
            }

            Response.Headers["HX-Trigger"] = "jobPartyChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await _service.BuildEditVmAsync(id);
            if (vm == null)
                return NotFound();

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobPartyFormVm vm)
        {

            if (string.IsNullOrWhiteSpace(vm.ContactEmail))
            {
                vm.ContactEmail = null;
                ModelState.Remove(nameof(vm.ContactEmail));
            }


            if (!ModelState.IsValid)
            {
                await _service.FillLookupsAsync(vm);
                return PartialView("_Edit", vm);
            }

            var result = await _service.UpdateAsync(vm, GetCurrentUserId());

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                await _service.FillLookupsAsync(vm);
                return PartialView("_Edit", vm);
            }

            Response.Headers["HX-Trigger"] = "jobPartyChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await _service.GetDetailsAsync(id);
            if (vm == null)
                return NotFound();

            return PartialView("_Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vm = await _service.GetDeleteVmAsync(id);
            if (vm == null)
                return NotFound();

            return PartialView("_Delete", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _service.DeleteAsync(id, GetCurrentUserId());

            if (!result.Success)
                return BadRequest(result.Error);

            Response.Headers["HX-Trigger"] = "jobPartyChanged";
            return Content(string.Empty);
        }

        private Guid GetCurrentUserId()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
            return _currentUserService.UserId;
        }

        [HttpGet]
        public async Task<IActionResult> SearchParties(
            string? term,
            string targetHiddenInputId = "CPartyId",
            string targetTextInputId = "CPartySearchText",
            CancellationToken cancellationToken = default)
        {
            var items = await _jobService.SearchPartiesAsync(term);

            ViewBag.TargetHiddenInputId = targetHiddenInputId;
            ViewBag.TargetTextInputId = targetTextInputId;

            return PartialView("_PartySearchResults", items);
        }



    }
}
