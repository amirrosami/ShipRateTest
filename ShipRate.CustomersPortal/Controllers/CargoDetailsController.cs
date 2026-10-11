using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebAppCustomers.Controllers
{
    public class CargoDetailsController : ApplicationController
    {
        private readonly ICargoDetailsService _service;
        private readonly ICurrentUserService _currentUserService;

        public CargoDetailsController(ICargoDetailsService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Panel(CargoDetailFilterVm filter)
        {
            //var items = await _service.GetListAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_CargoDetailsPanel", new List<CargoDetailListItemVm>());
        }

        [HttpGet]
        public async Task<IActionResult> List(CargoDetailFilterVm filter)
        {
            var items = await _service.GetListAsync(filter);

            ViewBag.Filter = filter;

            return PartialView("_CargoDetailsList", items);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CargoDetailFilterVm filter)
        {
            if (!filter.HasParent)
                return BadRequest("Parent context is required.");

            var model = await _service.BuildCreateModelAsync(filter);

            return PartialView("~/Views/CargoDetails/_Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoDetailFormVm model)
        {
            if (!model.ToFilter().HasParent)
                ModelState.AddModelError("", "Parent context is required.");

            if (!ModelState.IsValid)
            {
                await _service.FillLookupsAsync(model);
                Response.StatusCode = 422;
                return PartialView("_Create", model);
            }

            await _service.CreateAsync(model, GetCurrentUserId());



            HxTrigger(new { closeAppModalSecond = true, cargoDetailsChanged2 = true, toastSuccess = "Created Successfully" });

            //var items = await _service.GetListAsync(model.ToFilter());
            ViewBag.Filter = model.ToFilter();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.BuildEditModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CargoDetailFormVm model)
        {
            if (!ModelState.IsValid)
            {
                await _service.FillLookupsAsync(model);
                Response.StatusCode = 422;
                return PartialView("_Edit", model);
            }

            var updated = await _service.UpdateAsync(model, GetCurrentUserId());

            if (!updated)
                return NotFound();

            HxTrigger(new { cargoDetailsChanged2 = true, closeAppModalSecond = true, toastSuccess = "Edited Successfully" });


            //var items = await _service.GetListAsync(model.ToFilter());
            //ViewBag.Filter = model.ToFilter();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _service.GetDetailsAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _service.BuildDeleteModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var filter = await _service.DeleteAsync(id, GetCurrentUserId());

            if (filter == null)
                return NotFound();

            HxTrigger(new { cargoDetailsChanged2 = true, closeAppModalSecond = true, toastSuccess = "Deleted Successfully" });

            var items = await _service.GetListAsync(filter);
            ViewBag.Filter = filter;

            return Ok();
        }
        private Guid GetCurrentUserId()
        {
            //var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (Guid.TryParse(value, out var userId))
            //    return userId;
            //return Guid.Empty;
            return _currentUserService.UserId;
        }
    }
}
