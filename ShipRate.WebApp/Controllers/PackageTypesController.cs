using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class PackageTypesController : Controller
    {
        private readonly IPackageTypeService _service;
        private readonly ApplicationDbContext _context;
        private readonly IJobService _jobService;

        public PackageTypesController(IPackageTypeService service, ApplicationDbContext context, IJobService jobService)
        {
            _service = service;
            _context = context;
            _jobService = jobService;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }

        private async Task<PagedResult<PackageTypeDto>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.PackageTypes
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.PackageTypeCode.Contains(dto.SearchTerm) || x.PackageTypeName.Contains(dto.SearchTerm) || x.PackageTypeDescription.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new PackageTypeDto
            {
                Id = x.Id,
                PackageTypeCode = x.PackageTypeCode,
                PackageTypeName = x.PackageTypeName,
                PackageTypeDescription = x.PackageTypeDescription
            }).ToListAsync();

            return new PagedResult<PackageTypeDto>()
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


        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "PackageTypes/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>
            {
                new GridColumn("PackageTypeCode","Package Type Code"),new GridColumn("PackageTypeName","Package Type Name") ,new GridColumn("PackageTypeDescription","Description")
            };
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new PackageTypeCreateDto { IsActive = true });
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageTypeCreateDto dto)
        {
            // Check unique code




            ModelState.Clear();

            if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode))
            {
                ModelState.AddModelError("Code", "This code already exists.");
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                return PartialView("_CreateEdit", dto);
            }
            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";

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

            var dto = new PackageTypeCreateDto
            {
                Id = entity.Id,
                PackageTypeCode = entity.PackageTypeCode,
                PackageTypeName = entity.PackageTypeName,
                PackageTypeDescription = entity.PackageTypeDescription,
                ENSCode = entity.ENSCode,
                NumericCode = entity.NumericCode,
                Sequence = entity.Sequence,
                IsActive = entity.IsActive
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", dto);
        }




        // POST: /PackageTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PackageTypeCreateDto dto)
        {
            ModelState.Clear();
            if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode, dto.Id))
            {
                ModelState.AddModelError("Code", "This code already exists.");
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Edit";

                return PartialView("_CreateEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Edit";
                return PartialView("_CreateEdit", dto);
            }


            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }

        // GET: /PackageTypes/DeleteConfirm/5

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // var item = await _service.GetByIdAsync(id);
            //  if (item == null) return NotFound();
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
                    gridChanged = false,
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




        private async Task<IActionResult> CheckCode(string code, int? id)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Content("");

            var isUnique = await _service.IsCodeUniqueAsync(code.Trim().ToUpper(), id);
            if (isUnique)
                return Content("<span class=\"text-success small\"><i class=\"bi bi-check-circle\"></i> Available</span>");
            else
                return Content("<span class=\"text-danger small\"><i class=\"bi bi-x-circle\"></i> Already exists</span>");
        }


        [HttpGet]
        public async Task<IActionResult> SearchPackageTypes(
             string? term,
             string targetHiddenInputId = "PackageTypeId",
             string targetTextInputId = "PackageTypeSearchText",
             CancellationToken cancellationToken = default)
        {
            var items = await _jobService.SearchPackageTypesAsync(term);

            ViewBag.TargetHiddenInputId = targetHiddenInputId;
            ViewBag.TargetTextInputId = targetTextInputId;

            return PartialView("_PackageTypeSearchResults", items);
        }

        [HttpGet]
        public async Task<IActionResult> GetPackageTypesJson(string? q, int take = 20)
        {
            var ports = await _service.SearchPackageTypesAsync(q, take);
            var results = ports.Select(p => new
            {
                id = p.Value,
                text = p.Text
            });
            return Json(results);
        }
    }
}
