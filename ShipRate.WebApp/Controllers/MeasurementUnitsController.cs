using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class MeasurementUnitsController : Controller
    {
        private readonly IMeasurementUnitService _service;
        private readonly ApplicationDbContext _context;
        public MeasurementUnitsController(IMeasurementUnitService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }
        private async Task<PagedResult<MeasurementUnitDTO>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.MeasurementUnits
                .Where(x => !x.IsDeleted)
                .Include(x=>x.MeasurementUnitCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.MeasurementUnitName.Contains(dto.SearchTerm) || x.MeasurementUnitLocalName.Contains(dto.SearchTerm) || x.MeasurementUnitCategory.MeasurementUnitCategoryName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new MeasurementUnitDTO
            {
                Id = x.Id,
                IsActive = x.IsActive,
                MeasurementUnitName = x.MeasurementUnitName,
                MeasurementUnitLocalName = x.MeasurementUnitLocalName,
                MeasurementUnitCategoryName = x.MeasurementUnitCategory.MeasurementUnitCategoryName
            }).ToListAsync();

            return new PagedResult<MeasurementUnitDTO>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection,
                TotalCount = total,
            };

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "MeasurementUnits/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }


        private List<GridColumn> GetGridColumns()
        {
            var result = new List<GridColumn>
            {
                new GridColumn("MeasurementUnitCategoryName","Unit Category"),
                new GridColumn("MeasurementUnitName","Unit Name"),
                new GridColumn("MeasurementUnitLocalName","Unit Local Name"),
                new GridColumn("IsActive","Is Active")
            };
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _service.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "MeasurementUnitCategoryName");
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new MeasurementUnitCreateDTO { IsActive =true});
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeasurementUnitCreateDTO dto)
        {
            // Check unique code
            //if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode))
            //{
            //    ModelState.AddModelError("Code", "This code already exists.");
            //}

            //if (!ModelState.IsValid)
            //{
            //    Response.Headers.Append("HX-Retarget", "#modal-body");
            //    Response.Headers.Append("HX-Reswap", "innerHTML");
            //    return PartialView("_FormCreate", dto);
            //}

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                var categories = await _service.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "MeasurementUnitCategoryName");
                return PartialView("_CreateEdit", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                var categories = await _service.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "MeasurementUnitCategoryName");
                return PartialView("_CreateEdit", dto);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });

            return Content("");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var categories = await _service.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "MeasurementUnitCategoryName");

            var dto = new MeasurementUnitCreateDTO
            {
                Id = entity.Id,
                CategoryId = entity.MeasurementUnitCategoryId,
                UnitName = entity.MeasurementUnitName,
                UnitLocalName = entity.MeasurementUnitLocalName,
                IsActive = entity.IsActive
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MeasurementUnitCreateDTO dto)
        {
           
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Edit";
                var categories = await _service.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "MeasurementUnitCategoryName");
                return PartialView("_CreateEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Edit";
                var categories = await _service.GetCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "MeasurementUnitCategoryName");
                return PartialView("_CreateEdit", dto);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = await _service.GetByIdAsync(id);
            //if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", id);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChanged = true,
                    toastError = error
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Deleted Sussefully"
            });
            return Content("");
        }

    }
}
