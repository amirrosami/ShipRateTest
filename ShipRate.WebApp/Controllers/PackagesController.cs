using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class PackagesController : Controller
    {
        private readonly IPackagesService _service;
        private readonly ICurrentUserService _currentUserService;

        public PackagesController(IPackagesService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }
        private Guid GetCurrentUserId()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
            return _currentUserService.UserId;
        }
        [HttpGet]
        public async Task<IActionResult> Panel(Guid? rateRequestId, Guid? jobId, Guid? bookingId)
        {
            var filter = new RateRequestPackageFilterVm
            {
                RateRequestId = rateRequestId,
                JobId = jobId,
                BookingId = bookingId
            };

            var items = await _service.GetListAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_PackagesPanel", items);
        }

        [HttpGet]
        public async Task<IActionResult> List(Guid? rateRequestId, Guid? jobId, Guid? bookingId)
        {
            var filter = new RateRequestPackageFilterVm
            {
                RateRequestId = rateRequestId,
                JobId = jobId,
                BookingId = bookingId
            };

            var items = await _service.GetListAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_PackagesList", items);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? rateRequestId, Guid? jobId, Guid? bookingId)
        {
            var vm = await _service.BuildCreateVmAsync(new RateRequestPackageFilterVm
            {
                RateRequestId = rateRequestId,
                JobId = jobId,
                BookingId = bookingId
            });

            return PartialView("_Create", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RateRequestPackageFormVm model)
        {
            if (!ModelState.IsValid)
            {
                await _service.FillLookupsAsync(model);
                return PartialView("_Create", model);
            }

            var result = await _service.CreateAsync(model, GetCurrentUserId());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Create failed.");
                await _service.FillLookupsAsync(model);
                return PartialView("_Create", model);
            }

            Response.Headers["HX-Trigger"] = "packagesChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await _service.BuildEditVmAsync(id);
            if (vm == null) return NotFound();

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RateRequestPackageFormVm model)
        {
            if (!ModelState.IsValid)
            {
                await _service.FillLookupsAsync(model);
                return PartialView("_Edit", model);
            }

            var result = await _service.UpdateAsync(model, GetCurrentUserId());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Update failed.");
                await _service.FillLookupsAsync(model);
                return PartialView("_Edit", model);
            }

            Response.Headers["HX-Trigger"] = "packagesChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var vm = await _service.GetDetailsAsync(id);
            if (vm == null) return NotFound();

            return PartialView("_Details", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var vm = await _service.GetDeleteVmAsync(id);
            if (vm == null) return NotFound();

            return PartialView("_Delete", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var result = await _service.DeleteAsync(id, GetCurrentUserId());
            if (!result.Success)
                return BadRequest(result.Error);

            Response.Headers["HX-Trigger"] = "packagesChanged";
            return Content(string.Empty);
        }
    }
}
