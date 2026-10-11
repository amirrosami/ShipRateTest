using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.TransitViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public DriverController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [Authorize("Transit.Drivers.View")]
        public IActionResult Index()
        {
            PopulateViewBags();
            return View();
        }

        // ───────── Grid (partial, HTMX) ─────────
        [Authorize("Transit.Drivers.View")]
        public async Task<IActionResult> Grid(DriverSearchVM search)
        {
            var query = _db.Drivers
                .Include(d => d.Person)
                .Include(d => d.Carrier)
                .ThenInclude(d => d.Person)
                .Where(d => !d.IsDeleted);

            // Filter: Driver Name (from Person)
            if (!string.IsNullOrWhiteSpace(search.DriverName))
            {
                var term = search.DriverName.Trim().ToLower();
                query = query.Where(d =>
                    (d.Person.FirstName + " " + d.Person.LastName).ToLower().Contains(term)
                );
            }

            // Filter: Carrier
            if (search.CarrierId.HasValue)
            {
                query = query.Where(d => d.CarrierId == search.CarrierId.Value);
            }

            // Filter: Active Status
            if (search.IsActive.HasValue)
            {
                //query = query.Where(d => d.IsActive == search.IsActive.Value);
            }

            var totalCount = await query.CountAsync();
            var pageSize = search.PageSize > 0 ? search.PageSize : 10;
            var page = search.Page > 0 ? search.Page : 1;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderBy(d => d.Person.FirstName).ThenBy(d => d.Person.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DriverVM
                {
                    Id = d.Id,
                    DriverName = (d.Person.FirstName + " " + d.Person.LastName).Trim(),
                    NationalCode = d.Person.NationalID,
                    Mobile = d.Person.Mobile,
                    CarrierName = d.Carrier != null ? d.Carrier.Person.PersonName : "—",
                    CarrierId = d.CarrierId,
                    PassportNo = d.PassportNo,
                    LicenseNumber = d.LicenseNumber,
                    SmartCardNo = d.SmartCardNo,
                    //IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            // Pass search params for pagination links
            ViewBag.Search = search;

            return PartialView("_Grid", items);
        }

        [Authorize("Transit.Drivers.Edit")]
        public IActionResult Create()
        {
            PopulateViewBags();
            return PartialView("_Form", new DriverFormVM { IsActive = true });
        }

        // ───────── Edit ─────────
        [Authorize("Transit.Drivers.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var d = await _db.Drivers
                .Include(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (d == null) return NotFound();

            PopulateViewBags();

            return PartialView("_Form", new DriverFormVM
            {
                Id = d.Id,
                CarrierId = d.CarrierId,
                PassportNo = d.PassportNo,
                LicenseNumber = d.LicenseNumber,
                SmartCardNo = d.SmartCardNo,
                //IsActive = d.IsActive,
                PersonName = (d.Person.FirstName + " " + d.Person.LastName).Trim()
            });
        }

        [HttpPost]
        [Authorize("Transit.Drivers.Edit")]
        public async Task<IActionResult> Save(DriverFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                return PartialView("_Form", vm);
            }

            // Check: Person must exist
            var personExists = await _db.Persons.AnyAsync(p => p.Id == vm.Id && !p.IsDeleted);
            if (!personExists)
            {
                ModelState.AddModelError("Id", "Selected person does not exist.");
                PopulateViewBags();
                return PartialView("_Form", vm);
            }

            var existing = await _db.Drivers.FirstOrDefaultAsync(d => d.Id == vm.Id);

            if (existing == null)
            {
                // Create new driver record
                var driver = new Driver
                {
                    Id = vm.Id,  // = Person Id
                    CarrierId = vm.CarrierId,
                    PassportNo = vm.PassportNo,
                    LicenseNumber = vm.LicenseNumber,
                    SmartCardNo = vm.SmartCardNo,
                    //IsActive = vm.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId  // TODO: current user
                };
                _db.Drivers.Add(driver);
            }
            else
            {
                // Update
                existing.CarrierId = vm.CarrierId;
                existing.PassportNo = vm.PassportNo;
                existing.LicenseNumber = vm.LicenseNumber;
                existing.SmartCardNo = vm.SmartCardNo;
                //existing.IsActive = vm.IsActive;
                existing.UpdatedAt = DateTime.Now;
                existing.EditUserId = _currentUserService.UserId; // TODO: current user
            }

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "closeModalAndRefresh";
            return Content("");
        }

        [Authorize("Transit.Drivers.Edit")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var d = await _db.Drivers.FirstOrDefaultAsync(x => x.Id == id);
            if (d != null)
            {
                d.IsDeleted = true;
                d.UpdatedAt = DateTime.Now;
                d.EditUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();
            }

            Response.Headers["HX-Trigger"] = "closeModalAndRefresh";
            return Content("");
        }

        // ───────── Search Persons (for Create — find persons not yet drivers) ─────────
        public async Task<IActionResult> SearchPersons(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return Json(new List<object>());

            var existingDriverIds = _db.Drivers
                .Where(d => !d.IsDeleted)
                .Select(d => d.Id);

            var results = await _db.Persons
                .Where(p => !p.IsDeleted
                    && !existingDriverIds.Contains(p.Id)
                    && (p.PersonName).Contains(term))
                .Take(15)
                .Select(p => new
                {
                    id = p.Id,
                    name = (p.PersonName).Trim(),
                    nationalCode = p.NationalID ?? ""
                })
                .ToListAsync();
            return Json(results);
        }
        private void PopulateViewBags()
        {
            ViewBag.Carriers = _db.Carriers
                .Include(x => x.Person)
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Person.PersonName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Person.PersonName
                })
                .ToList();
        }
    }
}
