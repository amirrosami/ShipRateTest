using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class BLSerialSeriesController : Controller
    {
        private readonly IBLSerialSeriesService _service;
        private readonly IJobLookupService _lookupservice;
        private readonly ICurrentUserService _currentUserService;

        public BLSerialSeriesController(IBLSerialSeriesService service, IJobLookupService lookupservice, ICurrentUserService currentUserService)
        {
            _service = service;
            _lookupservice = lookupservice;
            _currentUserService = currentUserService;
        }
        [Authorize("BaseOperations.SerialsAndHolograms.View")]
        public async Task<IActionResult> Index()
        {
            var vm = await _service.GetIndexViewModelAsync();
            return View(vm);
        }
        [HttpPost]
        [Authorize("BaseOperations.SerialsAndHolograms.View")]
        public async Task<IActionResult> SeriesGrid(BLSerialSeriesSearchDto search)
        {
            var series = await _service.SearchSeriesAsync(search);
            return PartialView("_SeriesGrid", series);
        }
        [HttpGet]
        [Authorize("BaseOperations.SerialsAndHolograms.View")]
        public async Task<IActionResult> BatchAssignGrid(Guid seriesId)
        {
            var list = await _service.GetBatchAssignsAsync(seriesId);
            ViewBag.SeriesId = seriesId;
            return PartialView("_BatchAssignGrid", list);
        }

        [HttpGet]
        [Authorize("BaseOperations.SerialsAndHolograms.View")]
        public async Task<IActionResult> SerialGrid(Guid seriesId)
        {
            var list = await _service.GetSerialsAsync(seriesId);
            return PartialView("_SerialGrid", list);
        }

        [HttpGet]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> EditSeries(Guid? id)
        {
            var dto = await _service.GetSeriesForEditAsync(id);
            if (dto == null) return NotFound();

            // برای dropdown CarrierBLType
            var indexVm = await _service.GetIndexViewModelAsync();
            ViewBag.CarrierBLTypes = indexVm.CarrierBLTypes;

            return PartialView("_EditSeriesModal", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> EditSeries(BLSerialSeriesEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                var indexVm = await _service.GetIndexViewModelAsync();
                ViewBag.CarrierBLTypes = indexVm.CarrierBLTypes;
                return PartialView("_EditSeriesModal", dto);
            }

            var currentUserId = GetCurrentUserId();
            await _service.CreateOrUpdateSeriesAsync(dto, currentUserId);
            return new EmptyResult();
        }

        [HttpGet]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> DeleteSeries(Guid id)
        {
            ViewBag.Id = id;
            return PartialView("_DeleteSeriesModal");
        }

        // Delete سری - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> ConfirmDeleteSeries(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            await _service.DeleteSeriesAsync(id, currentUserId);
            return new EmptyResult();
        }

        // Create/Edit BatchAssign - GET (Modal)
        [HttpGet]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> EditBatchAssign(Guid? id, Guid seriesId)
        {
            var dto = await _service.GetBatchAssignForEditAsync(id, seriesId);
            if (dto == null) return NotFound();

            // dropdown Agents: Persons با PersonalityTypeId مثلاً Agent
            var agents = await _lookupservice.GetCarriersAsync(null);
            ViewBag.Agents = agents;

            return PartialView("_EditBatchAssignModal", dto);
        }

        // Create/Edit BatchAssign - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> EditBatchAssign(BLSerialBatchAssignEditDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Agents = await _lookupservice.GetCarriersAsync(null);
                return PartialView("_EditBatchAssignModal", dto);
            }

            var currentUserId = GetCurrentUserId();
            await _service.CreateOrUpdateBatchAssignAsync(dto, currentUserId);

            return new EmptyResult();
        }

        // Delete BatchAssign - GET (Confirm Modal)
        [HttpGet]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public IActionResult DeleteBatchAssign(Guid id, Guid seriesId)
        {
            ViewBag.Id = id;
            ViewBag.SeriesId = seriesId;
            return PartialView("_DeleteBatchAssignModal");
        }

        // Delete BatchAssign - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.SerialsAndHolograms.Edit")]
        public async Task<IActionResult> ConfirmDeleteBatchAssign(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            await _service.DeleteBatchAssignAsync(id, currentUserId);
            return new EmptyResult();
        }
        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId;
        }
        //private async Task<IEnumerable<SelectListItem>> GetAgentsSelectListAsync()
        //{
        //    // اینجا می‌توانید فیلتر مناسب روی Persons بگذارید
        //    using var scope = HttpContext.RequestServices.CreateScope();
        //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //    var agents = await db.Persons
        //        .Where(p => !p.IsDeleted && p.IsActive /* && p.PersonalityTypeId == ... */)
        //        .OrderBy(p => p.PersonName)
        //        .Select(p => new SelectListItem
        //        {
        //            Value = p.Id.ToString(),
        //            Text = p.PersonName
        //        })
        //        .ToListAsync();

        //    return agents;
        //}
    }
}
