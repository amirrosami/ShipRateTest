using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Services;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class CargoDetailsController : Controller
    {
        private readonly ICargoDetailsService _service;
        private readonly IContainerRecommendationService _recommendationService;
        private readonly ICurrentUserService _currentUserService;

        public CargoDetailsController(ICargoDetailsService service, IContainerRecommendationService recommendationService, ICurrentUserService currentUserService)
        {
            _service = service;
            _recommendationService = recommendationService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Panel(CargoDetailFilterVm filter)
        {
            var items = await _service.GetListAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_CargoDetailsPanel", items);
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

            Response.Headers["HX-Trigger"] = "cargoDetailsChanged";

            var items = await _service.GetListAsync(model.ToFilter());
            ViewBag.Filter = model.ToFilter();

            return PartialView("_CargoDetailsPanel", items);
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

            Response.Headers["HX-Trigger"] = "cargoDetailsChanged";

            var items = await _service.GetListAsync(model.ToFilter());
            ViewBag.Filter = model.ToFilter();

            return PartialView("_CargoDetailsPanel", items);
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

            Response.Headers["HX-Trigger"] = "cargoDetailsChanged";

            var items = await _service.GetListAsync(filter);
            ViewBag.Filter = filter;

            return PartialView("_CargoDetailsPanel", items);
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId;
        }

        [HttpGet]
        public async Task<IActionResult> Recommend(CargoDetailFilterVm vm)
        {
            var items = await _service.GetListAsync(vm);

            var totalVolumeCbm = items.Sum(x => (decimal?)x.Volume) ?? 0m;

            if (totalVolumeCbm == 0m)
            {
                var result2 = new ContainerRecommendationResult { ErrorMessage = "Volume is not Valid" };
                return PartialView("~/Views/CargoDetails/_RecommendationResult.cshtml", result2);
            }




            CargoDetailsInput input = new CargoDetailsInput();
            input.TotalVolumeCbm = items.Sum(x => (decimal)x.Volume);
            input.RequiresTemperatureControl = items.Where(x => x.IsTemperatureControlled == true).Any();
            input.TotalWeightKg = items.Sum(x => (decimal)x.GrossWeight);
            var result = await _recommendationService.RecommendContainer(input);
            return PartialView("~/Views/CargoDetails/_RecommendationResult.cshtml", result);



        }

        [HttpGet]
        public async Task<IActionResult> PushDataFromJobToBooking(CargoDetailFilterVm vm)
        {
            var b = await _service.PushDataFromJobToBookingAsync(vm);
            Response.Headers["HX-Trigger"] = "cargoDetailsChanged";
            return NoContent();

        }

        [HttpGet]
        public async Task<IActionResult> PushDataFromJobToBL(CargoDetailFilterVm vm)
        {
            var b = await _service.PushDataFromJobToBLAsync(vm);
            var items = await _service.GetListAsync(vm);
            ViewBag.Filter = vm;
            return PartialView("_CargoDetailsPanel", items);
        }
    }
}
