using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.Location;
using ShipRate.Application.Validations.Custom;
using ShipRate.Application.Validations.Domain;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.Location;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService currentUserService;
        private readonly IJobService _jobService;
        private readonly ILocationService _locationService;

        public LocationsController(IBaseDataService basedataservice, ApplicationDbContext context, ICurrentUserService currentUserService, IJobService jobService, ILocationService locationService)
        {
            _basedataservice = basedataservice;
            _context = context;
            this.currentUserService = currentUserService;
            _jobService = jobService;
            _locationService = locationService;
        }

        [Authorize(Policy = "BaseInfo.Locations.View")]
        [HttpGet]
        [IgnoreAntiforgeryToken] // برای GET نیازی نیست
        public async Task<IActionResult> Index()
        {
            //throw new GeneralAppException("custom error  in serverside");

            var locationTypes = await _context.LocationTypes.Where(x => !x.IsDeleted).Select(x => new { Id = x.Id, LocationTypeName = x.LocationTypeName }).ToListAsync();
            ViewBag.LocationTypes = new SelectList(locationTypes, "Id", "LocationTypeName");
            var result = await getFilteredList(new PagedRequestDto(), 0);
            return View(result);
        }



        private async Task<PagedResult<LocationListItemViewModel>> getFilteredList(PagedRequestDto dto, int locationType)
        {
            var data = _context.Locations
                .Include(x => x.LocationType)
                .Include(x => x.Country)
                .Include(X => X.ParentLocation)
                .Include(X => X.CityType)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.LocationCode.Contains(dto.SearchTerm) || x.LocationName.Contains(dto.SearchTerm) || x.Country.CountryName.Contains(dto.SearchTerm));
            }

            if (locationType != 0)
            {
                data = data.Where(x => x.LocationTypeId == locationType);
            }
            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new LocationListItemViewModel
            {
                LocationId = x.Id,
                LocationName = x.LocationName,
                LocationTypeName = x.LocationType.LocationTypeName,
                LocationLocalName = x.LocationLocalName,
                CountryName = x.Country.CountryName,
                CityTypeName = x.CityType.CityTypeName,
                ParentLocationName = x.ParentLocation.LocationName,
                LocationCode = x.LocationCode
            }).ToListAsync();

            return new PagedResult<LocationListItemViewModel>()
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

        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request, int locationType)
        {
            var result = await getFilteredList(request, locationType);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Locations/List",
                GridPartialName = "_MainGrid",
                columns = GetGridColumns()
            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
                new GridColumn { DisplayName = "Code", Name = "LocationCode" }, new GridColumn { DisplayName = "Location Name", Name = "LocationName" }, new GridColumn { Name = "LocationTypeName", DisplayName = "Location Type" }, new GridColumn { Name = "CountryName", DisplayName = "Country" } ,new GridColumn{ Name="ParentLocationName" ,DisplayName="Parent Location"} };
        }


        private async Task PopulateDropdowns(LocationCreateEditViewModel vm)
        {
            vm.LocationTypes = await _context.LocationTypes
                .OrderBy(t => t.Id)
                .Select(t => new SelectListItem(t.LocationTypeName, t.Id.ToString()))
                .ToListAsync();

            vm.Countries = await _context.Countries
                .OrderBy(c => c.Id)
                .Select(c => new SelectListItem(c.CountryName, c.Id.ToString()))
                .ToListAsync();

            vm.CityTypes = await _context.CityTypes
                .OrderBy(c => c.Id)
                .Select(c => new SelectListItem(c.CityTypeName, c.Id.ToString()))
                .ToListAsync();

            vm.DepotTypes = await _context.DepotTypes
                .OrderBy(d => d.Id)
                .Select(d => new SelectListItem(d.DepotTypeName, d.Id.ToString()))
                .ToListAsync();

            vm.TerminalTypes = await _context.TerminalTypes
                .OrderBy(t => t.Id)
                .Select(t => new SelectListItem(t.TerminalTypeName, t.Id.ToString()))
                .ToListAsync();
        }


        [Authorize(Policy = "BaseInfo.Locations.Edit")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var createVm = new LocationCreateEditViewModel() { };
            await PopulateDropdowns(createVm);
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", createVm);
        }






        [Authorize(Policy = "BaseInfo.Locations.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationCreateEditViewModel vm)
        {
            ModelState.Clear();
            //if (!ModelState.IsValid)
            //{
            //    await PopulateDropdowns(vm);
            //    Response.StatusCode = 422;
            //    ViewBag.CreateEdit = "Create";
            //    return PartialView("_CreateEdit", vm);
            //}

            var entity = new Domain.Entities.BaseInfo.Locations.Location
            {
                Id = Guid.CreateVersion7(),
                LocationCode = vm.LocationCode ?? "",
                LocationName = vm.LocationName.Trim(),
                LocationLocalName = vm.LocationLocalName?.Trim(),
                LocationTypeId = vm.LocationTypeId,
                CountryId = (int)vm.CountryId,
                CityTypeId = vm.CityTypeId,
                DepotTypeId = vm.DepotTypeId,
                TerminalTypeId = vm.TerminalTypeId,
                IATACode = vm.IATACode?.Trim(),
                ICAOCode = vm.ICAOCode?.Trim(),
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CountryId2 = vm.CountryId2,
                ParentLocationId = vm.ParentLocationId == Guid.Empty ? null : vm.ParentLocationId,
                Address = vm.Address,
                Phone = vm.Phone,
                Email = vm.Email,
                CreateUserId = currentUserService.UserId

            };

            var validationResult = new LoactionValidator().Validate(entity);
            if (!validationResult.IsValid)
            {
                await PopulateDropdowns(vm);
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                ModelState.AddModelError("", validationResult.GetErrorsAsString());
                return PartialView("_CreateEdit", vm);
            }

            if (!string.IsNullOrEmpty(vm.LocationCode))
            {
                if (_context.Locations.Where(x => x.LocationCode == vm.LocationCode && !x.IsDeleted).Any())
                {
                    await PopulateDropdowns(vm);
                    Response.StatusCode = 422;
                    ViewBag.CreateEdit = "Create";
                    ModelState.AddModelError("", "This Location Code already exists.");
                    return PartialView("_CreateEdit", vm);
                }
            }
            var success = false;
            _context.Locations.Add(entity);
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
                ModelState.AddModelError("", "Error in create new Location");
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                await PopulateDropdowns(vm);
                return PartialView("_CreateEdit", vm);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var location = await _context.Locations
               .Include(l => l.LocationType)
               .Include(l => l.Country)
               // .Include(l => l.)
               .Include(l => l.CityType)
               .Include(l => l.ParentLocation)
               .Include(l => l.DepotType)
               .Include(l => l.TerminalType)
               .Where(l => l.Id == id && !l.IsDeleted)
               .FirstOrDefaultAsync();

            if (location == null)
                return NotFound();

            //var childCount = await _context.Locations
            //    .CountAsync(l => l.ParentLocationId == id && !l.IsDeleted);
            var childCount = 112;

            var vm = new LocationDetailViewModel
            {
                LocationId = location.Id,
                LocationCode = location.LocationCode,
                LocationName = location.LocationName,
                LocationLocalName = location.LocationLocalName,
                LocationTypeId = location.LocationTypeId,
                LocationTypeName = location.LocationType?.LocationTypeName,
                CountryName = location.Country?.CountryName,
                //CountryName2 = location.Country2?.Name,
                //CityTypeName = location.CityType?.CityTypeName,
                ParentLocationName = location.ParentLocation?.LocationName,
                DepotTypeName = location.DepotType?.DepotTypeName,
                TerminalTypeName = location.TerminalType?.TerminalTypeName,
                IATACode = location.IATACode,
                ICAOCode = location.ICAOCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                // Description = location,
                IsActive = location.IsActive,
                CreatedDate = location.CreatedAt,
                ModifiedDate = location.UpdatedAt,
                Address = location.Address,
                Phone = location.Phone,
                Email = location.Email,

                ChildLocationsCount = childCount
            };

            return PartialView("_Detail", vm);
        }
        [Authorize(Policy = "BaseInfo.Locations.Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var location = await _context.Locations
               .Where(l => l.Id == id && !l.IsDeleted)
               .FirstOrDefaultAsync();
            if (location == null)
                return NotFound();
            var vm = new LocationCreateEditViewModel
            {
                Id = location.Id,
                LocationCode = location.LocationCode,
                LocationName = location.LocationName,
                LocationLocalName = location.LocationLocalName,
                LocationTypeId = location.LocationTypeId,
                CountryId = location.CountryId,
                CountryId2 = location.CountryId2,
                CityTypeId = location.CityTypeId,
                ParentLocationId = location.ParentLocationId,
                DepotTypeId = location.DepotTypeId,
                TerminalTypeId = location.TerminalTypeId,
                IATACode = location.IATACode,
                ICAOCode = location.ICAOCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                CreatedDate = location.CreatedAt,
                ModifiedDate = location.UpdatedAt,
                Address = location.Address,
                Phone = location.Phone,
                Email = location.Email
            };
            await PopulateDropdowns(vm);
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", vm);
        }


        [Authorize(Policy = "BaseInfo.Locations.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LocationCreateEditViewModel vm)
        {


            if (await _context.Locations.AnyAsync(l =>
                    l.LocationCode == vm.LocationCode &&
                    l.Id != vm.Id &&
                    !l.IsDeleted))
            {
                ModelState.AddModelError(nameof(vm.LocationCode),
                    "This Location Code already exists.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return PartialView("_CreateEdit", vm);
            }

            var entity = await _context.Locations
                .Where(l => l.Id == vm.Id && !l.IsDeleted)
                .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();

            entity.LocationCode = vm.LocationCode.Trim();
            entity.LocationName = vm.LocationName.Trim();
            entity.LocationLocalName = vm.LocationLocalName?.Trim();
            entity.LocationTypeId = vm.LocationTypeId;
            entity.CountryId = vm.CountryId ?? 1;
            entity.CountryId2 = vm.CountryId2;
            entity.CityTypeId = vm.CityTypeId;
            entity.ParentLocationId = vm.ParentLocationId == Guid.Empty ? null : vm.ParentLocationId;
            entity.DepotTypeId = vm.DepotTypeId;
            entity.TerminalTypeId = vm.TerminalTypeId;
            entity.IATACode = vm.IATACode?.Trim();
            entity.ICAOCode = vm.ICAOCode?.Trim();
            entity.Latitude = vm.Latitude;
            entity.Longitude = vm.Longitude;
            entity.LocationCode = vm.LocationCode.Trim();
            entity.UpdatedAt = DateTime.Now;
            entity.Address = vm.Address;
            entity.Phone = vm.Phone;
            entity.Email = vm.Email;
            entity.EditUserId = currentUserService.UserId;
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });

            return Ok();
        }


        [Authorize(Policy = "BaseInfo.Locations.Edit")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }


        [Authorize(Policy = "BaseInfo.Locations.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {

            var entity = await _context.Locations
                 .Where(l => l.Id == id && !l.IsDeleted)
                 .FirstOrDefaultAsync();

            if (entity == null)
                return NotFound();

            // Guard: cannot delete if children exist
            var childCount = await _context.Locations
                .CountAsync(l => l.ParentLocationId == id && !l.IsDeleted);

            if (childCount > 0)
            {
                HxTrigger(new
                {
                    GridChanged = false,
                    toastError = $"This Location have {childCount} child Locations and cannot delete."
                });
                return Content("");
            }
            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.EditUserId = currentUserService.UserId;
            await _context.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = $"Location Deleted Successfully!"
            });
            return Content("");
        }

        [HttpGet]
        public async Task<IActionResult> SearchParentLocations(string term, int locationTypeId, int country, int cityType)
        {
            if (locationTypeId == 0 || locationTypeId == 1)
            {
                return Json("");
            }
            int searchlocationType = 0;
            if (locationTypeId == 2)
            {
                if (cityType == 1 || cityType == 3)
                {
                    searchlocationType = 1;
                }
                if (cityType == 2)
                {
                    searchlocationType = 2;
                }
            }
            else
            {
                searchlocationType = 2;
            }

            var locations = await _context.Locations
                .Where(x => x.LocationTypeId == searchlocationType)
                .Where(x => x.CountryId == country)
                .Where(x => x.LocationName.ToLower().Contains(term.ToLower()))
                .Select(x => new { id = x.Id, LocationName = x.LocationName })
                .ToListAsync();
            return Json(locations);
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }


        [HttpGet]
        public async Task<IActionResult> SearchLocations(
            string? term,
            string targetHiddenInputId = "CLocationId",
            string targetTextInputId = "CLocationSearchText",
            CancellationToken cancellationToken = default)
        {
            var items = await _jobService.SearchLocationsAsync(term);

            ViewBag.TargetHiddenInputId = targetHiddenInputId;
            ViewBag.TargetTextInputId = targetTextInputId;

            return PartialView("_LocationSearchResults", items);
        }


        [HttpGet]
        public async Task<IActionResult> SearchPorts(string search, string index, string portType)
        {
            if (string.IsNullOrWhiteSpace(search) || search.Length < 2)
                return Content(string.Empty);

            var normalized = search.Trim();

            var ports = await _context.Locations
                .AsNoTracking()
                .Where(p => p.LocationLocalName.Contains(normalized) ||
                            p.LocationName.Contains(normalized) ||
                            p.LocationCode.Contains(normalized))
                .OrderBy(p => p.LocationName)
                .Take(8)
                .Select(p => new PortSearchItemVm
                {
                    Id = p.Id,
                    LocationName = p.LocationName,
                    LocationCode = p.LocationCode
                })
                .ToListAsync();

            ViewBag.Index = index;
            ViewBag.PortType = portType;
            return PartialView("_PortSearchResults", ports);
        }

        [HttpGet]
        public async Task<IActionResult> GetPortsJson(string? q, int take = 20)
        {
            //var ports = await _locationService.SearchPortsAsync(q, take);
            var ports = await _locationService.SearchPortsUsingCacheAsync(q, take);
            var results = ports.Select(p => new
            {
                id = p.Value,
                text = p.Text
            });
            return Json(results);
        }
    }

    public class PortSearchItemVm
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; }
        public string LocationCode { get; set; }
    }

}
