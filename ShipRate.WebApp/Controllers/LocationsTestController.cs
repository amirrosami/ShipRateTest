using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.Location;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class LocationsTestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationsTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ══════════════════════════════════════════════════════
        //  INDEX
        // ══════════════════════════════════════════════════════
        public async Task<IActionResult> Index(LocationIndexViewModel vm)
        {
            var query = _context.Locations
                .Include(l => l.LocationType)
                .Include(l => l.Country)
                .Include(l => l.CityType)
                .Include(l => l.ParentLocation)
                .Where(l => !l.IsDeleted)
                .AsQueryable();

            // ── Search ──
            if (!string.IsNullOrWhiteSpace(vm.SearchTerm))
            {
                var term = vm.SearchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.LocationCode.ToLower().Contains(term) ||
                    l.LocationName.ToLower().Contains(term) ||
                    (l.IATACode != null && l.IATACode.ToLower().Contains(term))
                );
            }

            // ── Filters ──
            if (vm.FilterLocationTypeId.HasValue)
                query = query.Where(l => l.LocationTypeId == vm.FilterLocationTypeId.Value);

            if (vm.FilterCountryId.HasValue)
                query = query.Where(l => l.CountryId == vm.FilterCountryId.Value);

            if (vm.FilterIsActive.HasValue)
                query = query.Where(l => l.IsActive == vm.FilterIsActive.Value);

            // ── Sorting ──
            query = vm.SortColumn?.ToLower() switch
            {
                "locationname" => vm.SortDirection == "desc"
                    ? query.OrderByDescending(l => l.LocationName)
                    : query.OrderBy(l => l.LocationName),
                "locationtype" => vm.SortDirection == "desc"
                    ? query.OrderByDescending(l => l.LocationType.LocationTypeName)
                    : query.OrderBy(l => l.LocationType.LocationTypeName),
                "country" => vm.SortDirection == "desc"
                    ? query.OrderByDescending(l => l.Country != null ? l.Country.CountryName : "")
                    : query.OrderBy(l => l.Country != null ? l.Country.CountryName : ""),
                "isactive" => vm.SortDirection == "desc"
                    ? query.OrderByDescending(l => l.IsActive)
                    : query.OrderBy(l => l.IsActive),
                "createddate" => vm.SortDirection == "desc"
                    ? query.OrderByDescending(l => l.CreatedAt)
                    : query.OrderBy(l => l.CreatedAt),
                _ => vm.SortDirection == "desc"
                    ? query.OrderByDescending(l => l.LocationCode)
                    : query.OrderBy(l => l.LocationCode),
            };

            // ── Paging ──
            vm.TotalCount = await query.CountAsync();

            if (vm.PageNumber < 1) vm.PageNumber = 1;
            if (vm.PageNumber > vm.TotalPages && vm.TotalPages > 0)
                vm.PageNumber = vm.TotalPages;

            var items = await query
                .Skip((vm.PageNumber - 1) * vm.PageSize)
                .Take(vm.PageSize)
                .Select(l => new LocationListItemViewModel
                {
                    LocationId = l.Id,
                    LocationCode = l.LocationCode,
                    LocationName = l.LocationName,
                    LocationTypeName = l.LocationType.LocationTypeName,
                    CountryName = l.Country != null ? l.Country.CountryName : null,
                    //CityTypeName = l.CityType != null ? l.CityType.CityTypeName : null,
                    //ParentLocationName = l.ParentLocation != null ? l.ParentLocation.LocationName : null,
                    IsActive = l.IsActive,
                    CreatedDate = l.CreatedAt
                })
                .ToListAsync();

            vm.Locations = items;

            // Populate filter dropdowns
            vm.LocationTypes = await _context.LocationTypes
                .Where(x => x.IsActive)
                .OrderBy(t => t.Id)
                .Select(t => new DropdownItem { Value = t.Id, Text = t.LocationTypeName })
                .ToListAsync();

            vm.Countries = await _context.Countries
                .OrderBy(c => c.CountryName)
                .Select(c => new DropdownItem { Value = c.Id, Text = c.CountryName })
                .ToListAsync();

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        //  DETAIL
        // ══════════════════════════════════════════════════════
        public async Task<IActionResult> Detail(Guid id)
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
                //CreatedBy = location.,
                //   ModifiedBy = location.ModifiedBy,
                ChildLocationsCount = childCount
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        //  CREATE  (GET)
        // ══════════════════════════════════════════════════════
        public async Task<IActionResult> Create()
        {
            var vm = new LocationCreateViewModel();
            await PopulateDropdowns(vm);
            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        //  CREATE  (POST)
        // ══════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationCreateViewModel vm)
        {
            // Unique code check
            if (await _context.Locations.AnyAsync(l =>
                    l.LocationCode == vm.LocationCode && l.LocationTypeId == vm.LocationTypeId && !l.IsDeleted))
            {
                ModelState.AddModelError(nameof(vm.LocationCode),
                    "This Location Code already exists.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return View(vm);
            }

            var entity = new Location
            {
                LocationCode = vm.LocationCode.Trim(),
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
                CountryId2 = vm.CountryId2
            };

            _context.Locations.Add(entity);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Location \"{entity.LocationName}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════════
        //  EDIT  (GET)
        // ══════════════════════════════════════════════════════
        public async Task<IActionResult> Edit(Guid id)
        {
            var location = await _context.Locations
                .Where(l => l.Id == id && !l.IsDeleted)
                .FirstOrDefaultAsync();

            if (location == null)
                return NotFound();

            var vm = new LocationEditViewModel
            {
                LocationId = location.Id,
                LocationCode = location.LocationCode,
                LocationName = location.LocationName,
                LocationLocalName = location.LocationName,
                LocationTypeId = location.LocationTypeId,
                CountryId = location.CountryId,
                CountryId2 = location.CountryId2,
                CityTypeId = location.CityTypeId,
                //ParentLocationId = location.ParentLocationId,
                DepotTypeId = location.DepotTypeId,
                TerminalTypeId = location.TerminalTypeId,
                IATACode = location.IATACode,
                ICAOCode = location.ICAOCode,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                CreatedDate = location.CreatedAt,
                ModifiedDate = location.UpdatedAt
            };

            await PopulateDropdowns(vm);
            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        //  EDIT  (POST)
        // ══════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LocationEditViewModel vm)
        {
            if (id != vm.LocationId)
                return BadRequest();

            // Unique code check (exclude self)
            if (await _context.Locations.AnyAsync(l =>
                    l.LocationCode == vm.LocationCode &&
                    l.Id != vm.LocationId &&
                    !l.IsDeleted))
            {
                ModelState.AddModelError(nameof(vm.LocationCode),
                    "This Location Code already exists.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(vm);
                return View(vm);
            }

            var entity = await _context.Locations
                .Where(l => l.Id == id && !l.IsDeleted)
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
            //entity.ParentLocationId = vm.ParentLocationId;
            entity.DepotTypeId = vm.DepotTypeId;
            entity.TerminalTypeId = vm.TerminalTypeId;
            entity.IATACode = vm.IATACode?.Trim();
            entity.ICAOCode = vm.ICAOCode?.Trim();
            entity.Latitude = vm.Latitude;
            entity.Longitude = vm.Longitude;
            entity.LocationCode = vm.LocationCode.Trim();
            entity.UpdatedAt = DateTime.Now;
            // entity.UpdatedAt = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Location \"{entity.LocationName}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════════
        //  DELETE  (GET - Confirmation Page)
        // ══════════════════════════════════════════════════════
        public async Task<IActionResult> Delete(Guid id)
        {
            var location = await _context.Locations
                .Include(l => l.LocationType)
                .Include(l => l.Country)
                .Where(l => l.Id == id && !l.IsDeleted)
                .FirstOrDefaultAsync();

            if (location == null)
                return NotFound();

            var childCount = await _context.Locations
                .CountAsync(l => l.ParentLocationId == id && !l.IsDeleted);

            var vm = new LocationDeleteViewModel
            {
                LocationId = location.Id,
                LocationCode = location.LocationCode,
                LocationName = location.LocationName,
                LocationTypeName = location.LocationType?.LocationTypeName,
                CountryName = location.Country?.CountryName,
                IsActive = location.IsActive,
                ChildLocationsCount = childCount
            };

            return View(vm);
        }

        // ══════════════════════════════════════════════════════
        //  DELETE  (POST - Soft Delete)
        // ══════════════════════════════════════════════════════
        [HttpPost, ActionName("Delete")]
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
                TempData["ErrorMessage"] =
                    $"Cannot delete \"{entity.LocationName}\" because it has {childCount} child location(s). Remove or reassign them first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            // Soft delete
            entity.IsDeleted = true;
            entity.IsActive = false;
            // entity.ModifiedDate = DateTime.Now;
            // entity.ModifiedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Location \"{entity.LocationName}\" deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ══════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════
        private async Task PopulateDropdowns(LocationCreateViewModel vm)
        {
            vm.LocationTypes = await _context.LocationTypes
                .OrderBy(t => t.Id)
                .Select(t => new DropdownItem { Value = t.Id, Text = t.LocationTypeName })
                .ToListAsync();

            vm.Countries = await _context.Countries
                .OrderBy(c => c.Id)
                .Select(c => new DropdownItem { Value = c.Id, Text = c.CountryName })
                .ToListAsync();

            vm.CityTypes = await _context.CityTypes
                .OrderBy(c => c.Id)
                .Select(c => new DropdownItem { Value = c.Id, Text = c.CityTypeName })
                .ToListAsync();

            vm.DepotTypes = await _context.DepotTypes
                .OrderBy(d => d.Id)
                .Select(d => new DropdownItem { Value = d.Id, Text = d.DepotTypeName })
                .ToListAsync();

            vm.TerminalTypes = await _context.TerminalTypes
                .OrderBy(t => t.Id)
                .Select(t => new DropdownItem { Value = t.Id, Text = t.TerminalTypeName })
                .ToListAsync();

            //vm.ParentLocations = await _context.Locations
            //    .Where(l => !l.IsDeleted && l.IsActive)
            //    .OrderBy(l => l.LocationName)
            //    .Select(l => new DropdownItem<Guid> { Value = 1, Text = $"{l.LocationCode} - {l.LocationName}" })
            //    .ToListAsync();
        }
    }
}
