using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.Location;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class BlackListReasonController : Controller
    {
        private readonly IBlackListReasonService _service;
        private readonly ApplicationDbContext _context;

        public BlackListReasonController(IBlackListReasonService service, ApplicationDbContext context)
        {
            _service = service;
            _context = context;
        }


        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }



        private async Task<PagedResult<BlackListReasonDto>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.BlackListReasons
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.BlackListReasonName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new BlackListReasonDto
            {
                Id = x.Id,
                BlackListReasonName = x.BlackListReasonName,
                SystemDefined = x.SystemDefined,
                IsActive = x.IsActive,
            }).ToListAsync();

            return new PagedResult<BlackListReasonDto>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalCount = total,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection
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
                ListUrl = "BlackListReason/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>
            {
                new GridColumn("BlackListReasonName","Title") , new GridColumn("SystemDefined","System Defined"),
                new GridColumn("IsActive","Is Active")
            };
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new BlackListReasonCreateDto { IsActive = true });
        }


        // POST: /BlackListReasons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlackListReasonCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";

                return PartialView("_CreateEdit", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ViewBag.CreateEdit = "Create";
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
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

            var dto = new BlackListReasonCreateDto
            {
                Id = entity.Id,
                BlackListReasonName = entity.BlackListReasonName,
                IsActive = entity.IsActive,
                SystemDefined = entity.SystemDefined
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", dto);
        }




        // POST: /BlackListReasons/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlackListReasonCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
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

        // GET: /BlackListReasons/DeleteConfirm/5

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", item.Id);
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
                    GridChanged = false,
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
