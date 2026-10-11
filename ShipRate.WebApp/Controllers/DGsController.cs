using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DGsController : Controller
    {
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _context;


        public DGsController(IBaseDataService basedataservice, ApplicationDbContext context)
        {
            // _basedataservice = basedataservice;
            _context = context;
        }



        [HttpGet]
        [IgnoreAntiforgeryToken] // برای GET نیازی نیست
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }



        private async Task<PagedResult<DGListItem>> getFilteredList(PagedRequestDto dto)
        {
            var query = _context.DGs
              .Where(d => !d.IsDeleted)
              .Include(x => x.DGClass)
              .Include(x => x.DGPackingGroup)
              .Include(x => x.DGRegulationSet)
              .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.SearchTerm))
            {
                var term = dto.SearchTerm.Trim().ToLower();
                query = query.Where(d =>
                    d.DGName.ToLower().Contains(term) ||
                    d.DGLocalName.ToLower().Contains(term) ||
                    d.DGUnNumber.Contains(term) ||
                    d.DGSubClassNo.Contains(term) ||
                    d.DGClass.DGClassName.Contains(term)
                    );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.DGUnNumber)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .Select(x =>
                new DGListItem
                {
                    Id = x.Id,
                    DGClassName = x.DGClass.DGClassName,
                    DGSubClassNo = x.DGSubClassNo,
                    DGUnNumber = x.DGUnNumber,
                    DGName = x.DGName,
                    DGLocalName = x.DGLocalName,
                    DGPackingGroupCode = x.DGPackingGroup.DGPackingGroupCode,
                    DGRegulationSetName = x.DGRegulationSet.DGRegulationSetName,
                    IsActive = x.DGRegulationSet.IsActive
                })
                .ToListAsync();

            return new PagedResult<DGListItem>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalCount = totalCount,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection

            };

        }

        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "DGs/List",
                GridPartialName = "_DGGrid",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
                new GridColumn { DisplayName = "DGUnNumber", Name = "DGUnNumber" }, new GridColumn { DisplayName = "DGClassNo", Name = "DGClassNo" }, new GridColumn { Name = "DGClassName", DisplayName = "DGClassName" }, new GridColumn { Name = "DGSubClassNo", DisplayName = "DGSubClassNo" }, new GridColumn { Name = "DGName", DisplayName = "DGName" },
                new GridColumn { DisplayName = "DGPackingGroupCode", Name = "DGPackingGroupCode" },new GridColumn { DisplayName = "DGRegulationSetName", Name = "DGRegulationSetName" } ,new GridColumn
                {
                    Name="IsActive",DisplayName="IsActive"
                }
            };
        }

        private async Task PopulateDropdowns(DGFormViewModel vm)
        {
            vm.DGClasses = await GetDGClassOptions();
            vm.PackingGroups = await GetPackingGroupOptions();
            vm.RegulationSets = await GetRegulationSetOptions();
        }





        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new DGFormViewModel();

            await PopulateDropdowns(model);
            ViewBag.CreateEdit = "Create";
            return PartialView("_DGForm", model);
        }




        private async Task<List<DGClassOption>> GetDGClassOptions()
        {
            return await _context.DGClasses
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DGClassNo)
                .Select(c => new DGClassOption
                {
                    Id = c.Id,
                    Display = c.DGClassNo + " - " + c.DGClassName
                })
                .ToListAsync();
        }

        private async Task<List<DGPackingGroupOption>> GetPackingGroupOptions()
        {
            return await _context.DGPackingGroups
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.DGPackingGroupCode)
                .Select(p => new DGPackingGroupOption
                {
                    Id = p.Id,
                    Display = p.DGPackingGroupCode + " - " + p.DGPackingGroupDescription
                })
                .ToListAsync();
        }

        private async Task<List<DGRegulationSetOption>> GetRegulationSetOptions()
        {
            return await _context.DGRegulationSets
                .Where(r => r.IsActive && !r.IsDeleted)
                .OrderBy(r => r.DGRegulationSetName)
                .Select(r => new DGRegulationSetOption
                {
                    Id = r.Id,
                    Display = r.DGRegulationSetName
                })
                .ToListAsync();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DGFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;

                ViewBag.CreateEdit = "Create";
                return PartialView("_DGForm", vm);
            }

            var entity = new DG
            {
                Id = Guid.CreateVersion7(),
                DGClassId = vm.DGClassId,
                DGSubClassNo = vm.DGSubClassNo,
                DGUnNumber = vm.DGUnNumber,
                DGName = vm.DGName,
                DGLocalName = vm.DGLocalName,
                DGPackingGroupId = vm.DGPackingGroupId,
                DGRegulationSetId = vm.DGRegulationSetId,
                //IsActive = vm.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString())
            };

            var success = false;
            _context.DGs.Add(entity);
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
                ModelState.AddModelError("", "Error in create new DG");
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                return PartialView("_DGForm", vm);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });

            return Ok();
        }

        //[HttpGet]
        //public async Task<IActionResult> Details(Guid id)
        //{
        //    var location = await _context.Locations
        //       .Include(l => l.LocationType)
        //       .Include(l => l.Country)
        //       // .Include(l => l.)
        //       .Include(l => l.CityType)
        //       .Include(l => l.ParentLocation)
        //       .Include(l => l.DepotType)
        //       .Include(l => l.TerminalType)
        //       .Where(l => l.Id == id && !l.IsDeleted)
        //       .FirstOrDefaultAsync();

        //    if (location == null)
        //        return NotFound();

        //    //var childCount = await _context.Locations
        //    //    .CountAsync(l => l.ParentLocationId == id && !l.IsDeleted);
        //    var childCount = 112;

        //    var vm = new LocationDetailViewModel
        //    {
        //        LocationId = location.Id,
        //        LocationCode = location.LocationCode,
        //        LocationName = location.LocationName,
        //        LocationLocalName = location.LocationLocalName,
        //        LocationTypeId = location.LocationTypeId,
        //        LocationTypeName = location.LocationType?.LocationTypeName,
        //        CountryName = location.Country?.CountryName,
        //        //CountryName2 = location.Country2?.Name,
        //        //CityTypeName = location.CityType?.CityTypeName,
        //        ParentLocationName = location.ParentLocation?.LocationName,
        //        DepotTypeName = location.DepotType?.DepotTypeName,
        //        TerminalTypeName = location.TerminalType?.TerminalTypeName,
        //        IATACode = location.IATACode,
        //        ICAOCode = location.ICAOCode,
        //        Latitude = location.Latitude,
        //        Longitude = location.Longitude,
        //        // Description = location,
        //        IsActive = location.IsActive,
        //        CreatedDate = location.CreatedAt,
        //        ModifiedDate = location.UpdatedAt,
        //        Address = location.Address,
        //        Phone = location.Phone,
        //        Email = location.Email,

        //        ChildLocationsCount = childCount
        //    };

        //    return PartialView("_Detail", vm);
        //}

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _context.DGs.FindAsync(id);
            if (entity == null) return NotFound();

            var model = new DGFormViewModel
            {
                Id = entity.Id,
                DGClassId = entity.DGClassId,
                DGSubClassNo = entity.DGSubClassNo,
                DGUnNumber = entity.DGUnNumber,
                DGName = entity.DGName,
                DGLocalName = entity.DGLocalName,
                DGPackingGroupId = entity.DGPackingGroupId,
                DGRegulationSetId = entity.DGRegulationSetId,
                //IsActive = entity.IsActive
            };

            await PopulateDropdowns(model);
            ViewBag.CreateEdit = "Edit";
            return PartialView("_DGForm", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DGFormViewModel vm)
        {

            if (!ModelState.IsValid)
            {
                return PartialView("_DGForm", vm);
            }

            var entity = await _context.DGs.FindAsync(vm.Id);
            if (entity == null) return NotFound();

            entity.DGClassId = vm.DGClassId;
            entity.DGSubClassNo = vm.DGSubClassNo;
            entity.DGUnNumber = vm.DGUnNumber;
            entity.DGName = vm.DGName;
            entity.DGLocalName = vm.DGLocalName;
            entity.DGPackingGroupId = vm.DGPackingGroupId;
            entity.DGRegulationSetId = vm.DGRegulationSetId;
            //entity.IsActive = vm.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());

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

            var entity = await _context.DGs
                 .Where(l => l.Id == id && !l.IsDeleted)
                 .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();

            entity.IsDeleted = true;
            //entity.IsActive = false;
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = $"DG Deleted Successfully!"
            });
            return Content("");
        }


        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }
    }
}



//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ShipRate.Domain.Entities.BaseInfo.Cargos;
//using ShipRate.Infrastructure.Data;
//using ShipRate.WebApp.ViewModels;

//namespace ShipRate.WebApp.Controllers
//{
//    public class DGsController : Controller
//    {
//        private readonly ApplicationDbContext _db;

//        public DGsController(ApplicationDbContext db)
//        {
//            _db = db;
//        }

//        // GET: /DGs
//        public async Task<IActionResult> Index()
//        {
//            var model = await GetPaged(new DGListViewModel());
//            return View(model);
//        }

//        // GET: /DGs/List
//        public async Task<IActionResult> List(DGListViewModel vm)
//        {
//            var model = await GetPaged(vm);
//            return PartialView("_DGGrid", model);
//        }


//        public async Task<DGListViewModel> GetPaged(DGListViewModel vm)
//        {
//            var query = _db.DGs
//              .Where(d => !d.IsDeleted)
//              .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(vm.SearchTerm))
//            {
//                var term = vm.SearchTerm.Trim().ToLower();
//                query = query.Where(d =>
//                    d.DGName.ToLower().Contains(term) ||
//                    d.DGLocalName.ToLower().Contains(term) ||
//                    d.DGUnNumber.Contains(term) ||
//                    d.DGSubClassNo.Contains(term));
//            }

//            if (vm.DGClassId.HasValue && vm.DGClassId > 0)
//            {
//                query = query.Where(d => d.DGClassId == vm.DGClassId.Value);
//            }

//            var totalCount = await query.CountAsync();

//            var items = await query
//                .OrderBy(d => d.DGUnNumber)
//                .Skip((vm.Page - 1) * vm.PageSize)
//                .Take(vm.PageSize)
//                .Join(_db.DGClasses, d => d.DGClassId, c => c.Id, (d, c) => new { d, c })
//                .Join(_db.DGPackingGroups, x => x.d.DGPackingGroupId, p => p.Id, (x, p) => new { x.d, x.c, p })
//                .Join(_db.DGRegulationSets, x => x.d.DGRegulationSetId, r => r.Id, (x, r) => new DGListItem
//                {
//                    Id = x.d.Id,
//                    DGClassNo = x.c.DGClassNo,
//                    DGClassName = x.c.DGClassName,
//                    DGSubClassNo = x.d.DGSubClassNo,
//                    DGUnNumber = x.d.DGUnNumber,
//                    DGName = x.d.DGName,
//                    DGLocalName = x.d.DGLocalName,
//                    DGPackingGroupCode = x.p.DGPackingGroupCode,
//                    DGRegulationSetName = r.DGRegulationSetName,
//                    IsActive = x.d.IsActive
//                })
//                .ToListAsync();

//            var model = new DGListViewModel
//            {
//                Items = items,
//                Page = vm.Page,
//                PageSize = vm.PageSize,
//                TotalCount = totalCount,
//                SearchTerm = vm.SearchTerm,
//                DGClassId = vm.DGClassId,
//                DGClasses = await GetDGClassOptions()
//            };
//            return model;
//        }
//        // GET: /DGs/Create
//        public async Task<IActionResult> Create()
//        {
//            var model = new DGFormViewModel
//            {
//                DGClasses = await GetDGClassOptions(),
//                PackingGroups = await GetPackingGroupOptions(),
//                RegulationSets = await GetRegulationSetOptions()
//            };
//            model.DGUnNumber = "amirxxx";
//            ViewBag.CreateEdit = "Create";
//            return PartialView("_DGForm", model);
//        }

//        // POST: /DGs/Create
//        [HttpPost]
//        public async Task<IActionResult> Create(DGFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                model.DGClasses = await GetDGClassOptions();
//                model.PackingGroups = await GetPackingGroupOptions();
//                model.RegulationSets = await GetRegulationSetOptions();
//                return PartialView("_DGForm", model);
//            }

//            var entity = new DG
//            {
//                Id = Guid.CreateVersion7(),
//                DGClassId = model.DGClassId,
//                DGSubClassNo = model.DGSubClassNo,
//                DGUnNumber = model.DGUnNumber,
//                DGName = model.DGName,
//                DGLocalName = model.DGLocalName,
//                DGPackingGroupId = model.DGPackingGroupId,
//                DGRegulationSetId = model.DGRegulationSetId,
//                IsActive = model.IsActive,
//                IsDeleted = false,
//                CreatedAt = DateTime.Now,
//                CreateUserId = GetCurrentUserId()
//            };

//            _db.DGs.Add(entity);
//            await _db.SaveChangesAsync();

//            Response.Headers["HX-Trigger"] = "dgChanged";
//            return Content("");
//        }

//        // GET: /DGs/Edit/{id}
//        public async Task<IActionResult> Edit(Guid id)
//        {
//            var entity = await _db.DGs.FindAsync(id);
//            if (entity == null) return NotFound();

//            var model = new DGFormViewModel
//            {
//                Id = entity.Id,
//                DGClassId = entity.DGClassId,
//                DGSubClassNo = entity.DGSubClassNo,
//                DGUnNumber = entity.DGUnNumber,
//                DGName = entity.DGName,
//                DGLocalName = entity.DGLocalName,
//                DGPackingGroupId = entity.DGPackingGroupId,
//                DGRegulationSetId = entity.DGRegulationSetId,
//                IsActive = entity.IsActive,
//                DGClasses = await GetDGClassOptions(),
//                PackingGroups = await GetPackingGroupOptions(),
//                RegulationSets = await GetRegulationSetOptions()
//            };

//            return PartialView("_DGForm", model);
//        }

//        // POST: /DGs/Edit/{id}
//        [HttpPost]
//        public async Task<IActionResult> Edit(Guid id, DGFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                model.DGClasses = await GetDGClassOptions();
//                model.PackingGroups = await GetPackingGroupOptions();
//                model.RegulationSets = await GetRegulationSetOptions();
//                return PartialView("_DGForm", model);
//            }

//            var entity = await _db.DGs.FindAsync(id);
//            if (entity == null) return NotFound();

//            entity.DGClassId = model.DGClassId;
//            entity.DGSubClassNo = model.DGSubClassNo;
//            entity.DGUnNumber = model.DGUnNumber;
//            entity.DGName = model.DGName;
//            entity.DGLocalName = model.DGLocalName;
//            entity.DGPackingGroupId = model.DGPackingGroupId;
//            entity.DGRegulationSetId = model.DGRegulationSetId;
//            entity.IsActive = model.IsActive;
//            entity.UpdatedAt = DateTime.Now;
//            entity.EditUserId = GetCurrentUserId();

//            await _db.SaveChangesAsync();

//            Response.Headers["HX-Trigger"] = "dgChanged";
//            return Content("");
//        }

//        // DELETE: /DGs/Delete/{id}
//        [HttpGet]
//        public async Task<IActionResult> DeleteConfirm(Guid id)
//        {
//            return PartialView("_DeleteConfirm",id);
//        }
//        [HttpPost]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var entity = await _db.DGs.FindAsync(id);
//            if (entity == null) return NotFound();

//            entity.IsDeleted = true;
//            entity.UpdatedAt = DateTime.Now;
//            entity.EditUserId = GetCurrentUserId();

//            await _db.SaveChangesAsync();

//            Response.Headers["HX-Trigger"] = "dgChanged";
//            return Content("");
//        }

//        // ===== Helpers =====
//        private async Task<List<DGClassOption>> GetDGClassOptions()
//        {
//            return await _db.DGClasses
//                .Where(c => c.IsActive && !c.IsDeleted)
//                .OrderBy(c => c.DGClassNo)
//                .Select(c => new DGClassOption
//                {
//                    Id = c.Id,
//                    Display = c.DGClassNo + " - " + c.DGClassName
//                })
//                .ToListAsync();
//        }

//        private async Task<List<DGPackingGroupOption>> GetPackingGroupOptions()
//        {
//            return await _db.DGPackingGroups
//                .Where(p => p.IsActive && !p.IsDeleted)
//                .OrderBy(p => p.DGPackingGroupCode)
//                .Select(p => new DGPackingGroupOption
//                {
//                    Id = p.Id,
//                    Display = p.DGPackingGroupCode + " - " + p.DGPackingGroupDescription
//                })
//                .ToListAsync();
//        }

//        private async Task<List<DGRegulationSetOption>> GetRegulationSetOptions()
//        {
//            return await _db.DGRegulationSets
//                .Where(r => r.IsActive && !r.IsDeleted)
//                .OrderBy(r => r.DGRegulationSetName)
//                .Select(r => new DGRegulationSetOption
//                {
//                    Id = r.Id,
//                    Display = r.DGRegulationSetName
//                })
//                .ToListAsync();
//        }

//        private Guid GetCurrentUserId()
//        {
//            // جایگزین کنید با لاجیک واقعی احراز هویت
//            return Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
//        }
//    }
//}
