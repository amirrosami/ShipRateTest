using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Cargo;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CargoCommodityController : Controller
    {
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _context;


        public CargoCommodityController(IBaseDataService basedataservice, ApplicationDbContext context)
        {
            _basedataservice = basedataservice;
            _context = context;
        }



        [HttpGet]
        [IgnoreAntiforgeryToken] // برای GET نیازی نیست
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }



        private async Task<PagedResult<CargoCommodityDto>> getFilteredList(PagedRequestDto dto)
        {
            var data = _context.CargoCommodities
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.CargoCommodityName.Contains(dto.SearchTerm) || x.CargoCommodityLocalName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new CargoCommodityDto
            {
                CargoCommodityLocalName = x.CargoCommodityLocalName,
                CargoCommodityName = x.CargoCommodityName,
                Id = x.Id
            }).ToListAsync();

            return new PagedResult<CargoCommodityDto>()
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
                ListUrl = "CargoCommodity/List",
                GridPartialName = "_MainGrid",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
                new GridColumn { DisplayName = "Local Name", Name = "CargoCommodityLocalName" }, new GridColumn { DisplayName = "Name", Name = "CargoCommodityName" }};
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var createVm = new CargoCommodityDto();
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", createVm);
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoCommodityDto vm)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;

                ViewBag.CreateEdit = "Create";
                return PartialView("_CreateEdit", vm);
            }

            var entity = new CargoCommodity
            {
                Id = Guid.CreateVersion7(),
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CargoCommodityName = vm.CargoCommodityName,
                CargoCommodityLocalName = vm.CargoCommodityLocalName
            };

            var success = false;
            _context.CargoCommodities.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                success = true;
            }
            catch (Exception)
            {

                success = false;
            }



            if (!success)
            {
                ModelState.AddModelError("", "Error in create new Cargo Commodity");
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                return PartialView("_CreateEdit", vm);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully",
                cargoCommodityCreated = new
                {
                    id = entity.Id,
                    name = entity.CargoCommodityName
                }
            });

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var location = await _context.CargoCommodities
               .Where(l => l.Id == id && !l.IsDeleted)
               .FirstOrDefaultAsync();

            if (location == null)
                return NotFound();


            var vm = new CargoCommodityDto
            {
                Id = id,
                CargoCommodityLocalName = location.CargoCommodityLocalName,
                CargoCommodityName = location.CargoCommodityName,
            };

            return PartialView("_Detail", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var location = await _context.CargoCommodities
               .Where(l => l.Id == id && !l.IsDeleted)
               .FirstOrDefaultAsync();

            if (location == null)
                return NotFound();

            var vm = new CargoCommodityDto
            {
                Id = location.Id,
                CargoCommodityName = location.CargoCommodityName,
                CargoCommodityLocalName = location.CargoCommodityLocalName
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CargoCommodityDto vm)
        {


            if (!ModelState.IsValid)
            {
                return PartialView("_CreateEdit", vm);
            }

            var entity = await _context.CargoCommodities
                .Where(l => l.Id == vm.Id && !l.IsDeleted)
                .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();

            entity.CargoCommodityName = vm.CargoCommodityName;
            entity.CargoCommodityLocalName = vm.CargoCommodityLocalName;
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });

            return Ok();
        }



        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {

            var entity = await _context.CargoCommodities
                 .Where(l => l.Id == id && !l.IsDeleted)
                 .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();


            entity.IsDeleted = true;
            entity.IsActive = false;
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = $"Deleted Successfully!"
            });
            return Content("");
        }


        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        public async Task<IActionResult> SelectOptions(Guid? selectedId = null)
        {
            var items = await _basedataservice.GetCargoCommoditiesAsync();
            ViewBag.SelectedId = selectedId;
            return PartialView("_CargoCommodityOptions", items);
        }




        //private readonly ICargoCommodityService _service;
        //public CargoCommodityController(ICargoCommodityService service)
        //{
        //    _service = service;
        //}

        //public async Task<IActionResult> Lookup(string term)
        //{
        //    var response = new List<dynamic>() {
        //    new { LocalName = "amir", Display = "amir2" },
        //    new { LocalName = "hassan", Display = "hassan2" },
        //    new { LocalName = "ali", Display = "ali2" },
        //    new { LocalName = "mohammad", Display = "mohammad2" },
        //    new { LocalName = "zeynab", Display = "zeynab2" },
        //    new { LocalName = "reza", Display = "reza2" },
        //    new { LocalName = "test", Display = "test2" },
        //    new { LocalName = "mahdi", Display = "mahdi2" },
        //    new { LocalName = "ghader", Display = "ghader" },
        //    new { LocalName = "ghader2", Display = "ghader2" },
        //    new { LocalName = "ghader3", Display = "ghader3" },
        //    new { LocalName = "ghader4", Display = "ghader4" },
        //    new { LocalName = "ghader5", Display = "ghader5" },
        //    new { LocalName = "ghader6", Display = "ghader6" },
        //    };

        //    return Ok(response.Where(x => x.LocalName.Contains(term)));
        //}
        //public async Task<IActionResult> Index(CargoCommoditySearchDto searchDto, bool Partial = false)
        //{
        //    var result = await _service.GetPagedListAsync(searchDto);
        //    ViewBag.SearchDto = searchDto;
        //    //TempData["SuccessMessage"] = "";
        //    //TempData["ErrorMessage"] = "";
        //    if (Partial)
        //    {
        //        return PartialView("_Grid", result);
        //    }
        //    return View(result);
        //}

        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(CargoCommodityDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(dto);
        //    }
        //    try
        //    {
        //        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.CreateVersion7().ToString());
        //        await _service.CreateAsync(dto, userId);
        //        TempData["SuccessMessage"] = "Success";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        return View(dto);
        //    }
        //}

        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var dto = await _service.GetByIdAsync(id);
        //    if (dto == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(dto);
        //}

        //[HttpPost]
        ////  [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(CargoCommodityDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(dto);
        //    }

        //    try
        //    {
        //        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // باید از احراز هویت بگیرید
        //        await _service.UpdateAsync(dto, userId);
        //        TempData["SuccessMessage"] = "Successfully edited";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError("", ex.Message);
        //        return View(dto);
        //    }
        //}

        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var dto = await _service.GetByIdAsync(id);
        //    if (dto == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(dto);
        //}

        //[HttpPost, ActionName("Delete")]
        //// [ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    try
        //    {
        //        await _service.DeleteAsync(id);
        //        TempData["SuccessMessage"] = "Successfully deleted";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        //public async Task<IActionResult> Details(Guid id)
        //{
        //    var dto = await _service.GetByIdAsync(id);
        //    if (dto == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(dto);
        //}
    }
}
