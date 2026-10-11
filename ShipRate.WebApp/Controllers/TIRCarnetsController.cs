using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces.Transit;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TIRCarnetsController : Controller
    {
        private readonly ITIRCarnetService _tirCarnetService;

        public TIRCarnetsController(ITIRCarnetService tirCarnetService)
        {
            _tirCarnetService = tirCarnetService;
        }

        // --------------------------------------------------------------------
        // Main Page
        // --------------------------------------------------------------------

        [HttpGet]
        [Authorize("Transit.TIRCarnets.View")]
        public async Task<IActionResult> Index([FromQuery] TIRCarnetSearchVm search)
        {
            var model = await _tirCarnetService.GetIndexVmAsync(search);
            return View(model);
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.View")]
        public async Task<IActionResult> Search([FromQuery] TIRCarnetSearchVm search)
        {
            var result = await _tirCarnetService.SearchAsync(search);
            return PartialView("_TIRCarnetsGrid", result);
        }

        // --------------------------------------------------------------------
        // TIR Carnet CRUD
        // --------------------------------------------------------------------

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> Create()
        {
            var model = await _tirCarnetService.GetCreateModelAsync();
            return PartialView("_TIRCarnetForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> Create(TIRCarnetCreateEditVm model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadCarnetLookupsForInvalidModel(model);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRCarnetForm", model);
            }

            try
            {
                await _tirCarnetService.CreateAsync(model);

                Response.Headers["HX-Trigger"] = "tirCarnetChanged";
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await ReloadCarnetLookupsForInvalidModel(model);

                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRCarnetForm", model);
            }
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _tirCarnetService.GetEditModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_TIRCarnetForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> Edit(TIRCarnetCreateEditVm model)
        {
            if (!model.Id.HasValue)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await ReloadCarnetLookupsForInvalidModel(model);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRCarnetForm", model);
            }

            try
            {
                var updated = await _tirCarnetService.UpdateAsync(model);

                if (!updated)
                    return NotFound();

                Response.Headers["HX-Trigger"] = "tirCarnetChanged";
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await ReloadCarnetLookupsForInvalidModel(model);

                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRCarnetForm", model);
            }
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _tirCarnetService.GetDeleteModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_TIRCarnetDelete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var deleted = await _tirCarnetService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            Response.Headers["HX-Trigger"] = "tirCarnetChanged";
            return NoContent();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> ChangeStatus(Guid id, int statusId)
        {
            var changed = await _tirCarnetService.ChangeStatusAsync(id, statusId);

            if (!changed)
                return BadRequest();

            Response.Headers["HX-Trigger"] = "tirCarnetChanged";
            return NoContent();
        }

        // --------------------------------------------------------------------
        // Assignments Panel
        // --------------------------------------------------------------------

        [HttpGet]
        [Authorize("Transit.TIRCarnets.View")]
        public async Task<IActionResult> Assignments(Guid tirCarnetId)
        {
            var model = await _tirCarnetService.GetAssignmentsPanelAsync(tirCarnetId);

            if (model == null)
                return NotFound();

            return PartialView("_AssignmentsPanel", model);
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> CreateAssignment(Guid tirCarnetId)
        {
            var model = await _tirCarnetService.GetAssignmentCreateModelAsync(tirCarnetId);

            if (model == null)
                return NotFound();

            return PartialView("_TIRAssignmentForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> CreateAssignment(TIRAssignmentCreateEditVm model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadAssignmentLookupsForInvalidModel(model);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRAssignmentForm", model);
            }

            try
            {
                await _tirCarnetService.CreateAssignmentAsync(model);

                Response.Headers["HX-Trigger"] = BuildAssignmentChangedTrigger(model.TIRCarnetId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await ReloadAssignmentLookupsForInvalidModel(model);

                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRAssignmentForm", model);
            }
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> EditAssignment(Guid id)
        {
            var model = await _tirCarnetService.GetAssignmentEditModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_TIRAssignmentForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> EditAssignment(TIRAssignmentCreateEditVm model)
        {
            if (!model.Id.HasValue)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await ReloadAssignmentLookupsForInvalidModel(model);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRAssignmentForm", model);
            }

            try
            {
                var updated = await _tirCarnetService.UpdateAssignmentAsync(model);

                if (!updated)
                    return NotFound();

                Response.Headers["HX-Trigger"] = BuildAssignmentChangedTrigger(model.TIRCarnetId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await ReloadAssignmentLookupsForInvalidModel(model);

                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRAssignmentForm", model);
            }
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> DeleteAssignment(Guid id)
        {
            var model = await _tirCarnetService.GetAssignmentDeleteModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_TIRAssignmentDelete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> DeleteAssignmentConfirmed(Guid id, Guid tirCarnetId)
        {
            var deleted = await _tirCarnetService.DeleteAssignmentAsync(id);

            if (!deleted)
                return NotFound();

            Response.Headers["HX-Trigger"] = BuildAssignmentChangedTrigger(tirCarnetId);
            return NoContent();
        }

        // --------------------------------------------------------------------
        // Usages Panel by Assignment
        // --------------------------------------------------------------------

        [HttpGet]
        [Authorize("Transit.TIRCarnets.View")]
        public async Task<IActionResult> Usages(Guid tirAssignmentId)
        {
            var model = await _tirCarnetService.GetUsagesPanelAsync(tirAssignmentId);

            if (model == null)
                return NotFound();

            return PartialView("_UsagesPanel", model);
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> CreateUsage(Guid tirAssignmentId)
        {
            var model = await _tirCarnetService.GetUsageCreateModelAsync(tirAssignmentId);

            if (model == null)
                return NotFound();

            return PartialView("_TIRUsageForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> CreateUsage(TIRUsageCreateEditVm model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadUsageLookupsForInvalidModel(model);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRUsageForm", model);
            }

            try
            {
                await _tirCarnetService.CreateUsageAsync(model);

                Response.Headers["HX-Trigger"] = BuildUsageChangedTrigger(model.TIRAssignmentId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await ReloadUsageLookupsForInvalidModel(model);

                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRUsageForm", model);
            }
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> EditUsage(Guid id)
        {
            var model = await _tirCarnetService.GetUsageEditModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_TIRUsageForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> EditUsage(TIRUsageCreateEditVm model)
        {
            if (!model.Id.HasValue)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await ReloadUsageLookupsForInvalidModel(model);
                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRUsageForm", model);
            }

            try
            {
                var updated = await _tirCarnetService.UpdateUsageAsync(model);

                if (!updated)
                    return NotFound();

                Response.Headers["HX-Trigger"] = BuildUsageChangedTrigger(model.TIRAssignmentId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await ReloadUsageLookupsForInvalidModel(model);

                Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                return PartialView("_TIRUsageForm", model);
            }
        }

        [HttpGet]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> DeleteUsage(Guid id)
        {
            var model = await _tirCarnetService.GetUsageDeleteModelAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_TIRUsageDelete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.TIRCarnets.Edit")]
        public async Task<IActionResult> DeleteUsageConfirmed(Guid id, Guid tirAssignmentId)
        {
            var deleted = await _tirCarnetService.DeleteUsageAsync(id);

            if (!deleted)
                return NotFound();

            Response.Headers["HX-Trigger"] = BuildUsageChangedTrigger(tirAssignmentId);
            return NoContent();
        }

        // --------------------------------------------------------------------
        // Private Helpers
        // --------------------------------------------------------------------

        private string BuildAssignmentChangedTrigger(Guid tirCarnetId)
        {
            return $$"""
        {
            "tirCarnetChanged": true,
            "tirAssignmentsChanged": {
                "tirCarnetId": "{{tirCarnetId}}"
            }
        }
        """;
        }

        private string BuildUsageChangedTrigger(Guid tirAssignmentId)
        {
            return $$"""
        {
            "tirUsagesChanged": {
                "tirAssignmentId": "{{tirAssignmentId}}"
            }
        }
        """;
        }

        private async Task ReloadCarnetLookupsForInvalidModel(TIRCarnetCreateEditVm model)
        {
            if (model.Id.HasValue)
            {
                var reloaded = await _tirCarnetService.GetEditModelAsync(model.Id.Value);
                if (reloaded != null)
                {
                    model.Countries = reloaded.Countries;
                    model.Associations = reloaded.Associations;
                    model.Currencies = reloaded.Currencies;
                    model.Statuses = reloaded.Statuses;
                }
            }
            else
            {
                var reloaded = await _tirCarnetService.GetCreateModelAsync();
                model.Countries = reloaded.Countries;
                model.Associations = reloaded.Associations;
                model.Currencies = reloaded.Currencies;
                model.Statuses = reloaded.Statuses;
            }
        }

        private async Task ReloadAssignmentLookupsForInvalidModel(TIRAssignmentCreateEditVm model)
        {
            TIRAssignmentCreateEditVm? reloaded;

            if (model.Id.HasValue)
                reloaded = await _tirCarnetService.GetAssignmentEditModelAsync(model.Id.Value);
            else
                reloaded = await _tirCarnetService.GetAssignmentCreateModelAsync(model.TIRCarnetId);

            if (reloaded != null)
            {
                model.Trucks = reloaded.Trucks;
                model.TIRCarnetNumber = reloaded.TIRCarnetNumber;
            }
        }

        private async Task ReloadUsageLookupsForInvalidModel(TIRUsageCreateEditVm model)
        {
            TIRUsageCreateEditVm? reloaded;

            if (model.Id.HasValue)
                reloaded = await _tirCarnetService.GetUsageEditModelAsync(model.Id.Value);
            else
                reloaded = await _tirCarnetService.GetUsageCreateModelAsync(model.TIRAssignmentId);

            if (reloaded != null)
            {
                model.Borders = reloaded.Borders;
                model.CustomsOffices = reloaded.CustomsOffices;
                model.InOutItems = reloaded.InOutItems;
                model.TIRCarnetId = reloaded.TIRCarnetId;
                model.TIRCarnetNumber = reloaded.TIRCarnetNumber;
                model.TruckDisplay = reloaded.TruckDisplay;
            }
        }
    }
}