using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class EventTypesController : Controller
    {
        private readonly IEventTypeService _eventTypeService;

        public EventTypesController(IEventTypeService eventTypeService)
        {
            _eventTypeService = eventTypeService;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var vm = new EventTypeManagementViewModel
            {
                Categories = await _eventTypeService.GetCategoriesAsync(),
                SelectedCategoryId = categoryId
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> List(int? categoryId, string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _eventTypeService.GetPagedAsync(new EventTypeSearchDto
            {
                CategoryId = categoryId,
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
            ViewBag.CategoryId = categoryId;
            return PartialView("_EventTypeGrid", result);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? categoryId)
        {
            var vm = await _eventTypeService.GetCreateFormAsync(categoryId);
            vm.CreateDto.Categories = vm.Categories;
            return PartialView("_CreateModal", vm.CreateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventTypeDto vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = (await _eventTypeService.GetCreateFormAsync(vm.CategoryId)).Categories;
                Response.Headers["HX-Retarget"] = "#modalContent";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateModal", vm);
            }

            var result = await _eventTypeService.CreateAsync(vm);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                vm.Categories = (await _eventTypeService.GetCreateFormAsync(vm.CategoryId)).Categories;
                Response.Headers["HX-Retarget"] = "#modalContent";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_CreateModal", vm);
            }

            Response.Headers["HX-Trigger"] = "eventTypeChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _eventTypeService.GetEditFormAsync(id);
            if (vm == null)
                return NotFound();

            vm.UpdateDto.Categories = vm.Categories;
            return PartialView("_EditModal", vm.UpdateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEventTypeDto vm)
        {
            if (!ModelState.IsValid)
            {
                var form = await _eventTypeService.GetEditFormAsync(vm.Id);
                if (form != null)
                    vm.Categories = form.Categories;

                Response.Headers["HX-Retarget"] = "#modalContent";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_EditModal", vm);
            }

            var result = await _eventTypeService.UpdateAsync(vm);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                var form = await _eventTypeService.GetEditFormAsync(vm.Id);
                if (form != null)
                    vm.Categories = form.Categories;

                Response.Headers["HX-Retarget"] = "#modalContent";
                Response.Headers["HX-Reswap"] = "innerHTML";
                return PartialView("_EditModal", vm);
            }

            Response.Headers["HX-Trigger"] = "eventTypeChanged";
            return Content(string.Empty);
        }

        [HttpGet]
        public IActionResult Delete(int id, int? categoryId, string? searchTerm, int pageNumber = 1)
        {
            ViewBag.Id = id;
            ViewBag.CategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.PageNumber = pageNumber;

            return PartialView("_DeleteModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventTypeService.DeleteAsync(id);
            Response.Headers["HX-Trigger"] = "eventTypeChanged";
            return Content(string.Empty);
        }
    }
}