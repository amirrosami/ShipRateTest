using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class TelexReleaseController : Controller
    {
        private readonly ITelexReleaseService _service;
        private readonly ICurrentUserService _currentUserService;

        public TelexReleaseController(ITelexReleaseService service, ICurrentUserService currentUserService)
        {
            _service = service;
            _currentUserService = currentUserService;
        }
        [Authorize("Operations.TelexReleases.View")]
        public async Task<IActionResult> Index(TelexReleaseSearchViewModel search)
        {
            var model = await _service.GetIndexAsync(search);
            return View(model);
        }

        [HttpGet]
        [Authorize("Operations.TelexReleases.View")]
        public async Task<IActionResult> Search(TelexReleaseSearchViewModel search)
        {
            var result = await _service.SearchAsync(search);
            ViewBag.Search = search;
            return PartialView("_Grid", result);
        }

        [HttpGet]
        [Authorize("Operations.TelexReleases.Edit")]
        public async Task<IActionResult> Create(Guid? blId, string? BLNo)
        {
            var model = await _service.GetCreateModelAsync(blId);
            model.BLNo = BLNo;
            return PartialView("_CreateEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.TelexReleases.Edit")]
        public async Task<IActionResult> Create(TelexReleaseCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //model.Statuses = await _service.GetStatusSelectListAsync();
                model.LineForwarderTypes = await _service.GetLineForwarderTypeSelectListAsync();
                return PartialView("_CreateEdit", model);
            }

            var userId = _currentUserService.UserId;
            await _service.CreateAsync(model, userId);
            Response.Headers["HX-Trigger"] = "telexReleaseChanged";
            return Content("");
        }
        [HttpGet]
        [Authorize("Operations.TelexReleases.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetEditModelAsync(id);
            if (model == null) return NotFound();

            return PartialView("_CreateEdit", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.TelexReleases.Edit")]
        public async Task<IActionResult> Edit(TelexReleaseCreateEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //model.Statuses = await _service.GetStatusSelectListAsync();
                model.LineForwarderTypes = await _service.GetLineForwarderTypeSelectListAsync();
                return PartialView("_CreateEdit", model);
            }
            var userId = _currentUserService.UserId;
            await _service.UpdateAsync(model, userId);
            Response.Headers["HX-Trigger"] = "telexReleaseChanged";
            return Content("");
        }
        [HttpGet]
        [Authorize("Operations.TelexReleases.View")]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _service.GetDetailsAsync(id);
            if (model == null) return NotFound();

            return PartialView("_Details", model);
        }

        [HttpGet]
        [Authorize("Operations.TelexReleases.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _service.GetDeleteModelAsync(id);
            if (model == null) return NotFound();

            return PartialView("_Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.TelexReleases.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userId = _currentUserService.UserId;
            await _service.DeleteAsync(id, userId);
            Response.Headers["HX-Trigger"] = "telexReleaseChanged";
            return Content("");
        }





        [HttpGet]
        [Authorize("Operations.TelexReleases.Approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var model = await _service.GetApproveModelAsync(id);
            if (model == null) return NotFound();
            return PartialView("_Approve", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.TelexReleases.Approve")]
        public async Task<IActionResult> ApproveConfirmed(Guid id)
        {
            var userId = _currentUserService.UserId; // replace with current user id
            await _service.ApproveAsync(id, userId);
            Response.Headers["HX-Trigger"] = "telexReleaseChanged";
            return Content("");
        }
        [HttpGet]
        public async Task<IActionResult> BLLookup(string? searchTerm)
        {
            var model = await _service.SearchBLAsync(searchTerm);
            return PartialView("_BLLookup", model);
        }
    }
}
