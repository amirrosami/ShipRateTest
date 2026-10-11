using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.TransitViewModels;
using System.Linq.Dynamic.Core;
using System.Text.Json;
namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class TruckController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public TruckController(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _db = context;
            _currentUser = currentUser;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");

        private void HxTrigger(object triggers)
        {
            Response.Headers.Append("HX-Trigger", JsonSerializer.Serialize(triggers));
        }

        [HttpGet]
        [Authorize("Transit.Trucks.View")]
        public async Task<IActionResult> Index()
        {
            var result = await GetFilteredList(new PagedRequestDto());
            return View(result);
        }

        [HttpPost]
        [Authorize("Transit.Trucks.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await GetFilteredList(request);

            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Truck/List",
                GridPartialName = "_List",
                columns = GetGridColumns()
            };

            return PartialView("_Grid", gridVm);
        }

        private async Task<ViewModels.PagedResult<TruckVM>> GetFilteredList(PagedRequestDto dto)
        {
            var query = _db.Trucks
                .Include(t => t.TruckType)
                .Include(t => t.Carrier).ThenInclude(t => t.Person)
                .Include(t => t.Owner)
                .Include(t => t.LastDriver)
                .Include(t => t.Country)
                .Where(t => !t.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.SearchTerm))
            {
                var term = dto.SearchTerm.Trim().ToLower();
                query = query.Where(t =>
                    t.TruckReferenceTitle.ToLower().Contains(term) ||
                    t.TruckPlateNumber.ToLower().Contains(term) ||
                    t.TrailerPlateNumber.ToLower().Contains(term) ||
                    t.TruckType.TruckTypeName.ToLower().Contains(term) ||
                    (t.Owner != null && (t.Owner.PersonName).ToLower().Contains(term)) ||
                    (t.Carrier != null && t.Carrier.Person.PersonName.ToLower().Contains(term)));
            }
            var selectQuery = query.Select(t => new TruckVM
            {
                Id = t.Id,
                TruckReferenceTitle = t.TruckReferenceTitle,
                TruckTypeName = t.TruckType.TruckTypeName,
                TruckPlateNumber = t.TruckPlateNumber,
                TrailerPlateNumber = t.TrailerPlateNumber,
                OwnerName = t.Owner != null ? t.Owner.PersonName : "—",
                CarrierName = t.Carrier != null ? t.Carrier.Person.PersonName : "—",
                LastDriverName = t.LastDriver != null ? t.LastDriver.FirstName + " " + t.LastDriver.LastName : "—",
                CountryName = t.Country.CountryName,
                BrandModel = t.Brand + " - " + t.Model,
                MaxPayloadKg = t.MaxPayloadKg,
                MaxVolumeCBM = t.MaxVolumeCBM,
                //IsActive = t.IsActive
            });

            if (!string.IsNullOrEmpty(dto.SortColumn) && !string.IsNullOrEmpty(dto.SortDirection))
            {
                selectQuery = selectQuery.OrderBy($"{dto.SortColumn}  {dto.SortDirection}");
            }

            int totalCount = await selectQuery.CountAsync();

            var items = await selectQuery
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return new ViewModels.PagedResult<TruckVM>
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                TotalCount = totalCount,
                SortDirection = dto.SortDirection,
                SortColumn = dto.SortColumn
            };
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>
            {
                new GridColumn("TruckReferenceTitle", "Reference"),
                new GridColumn("TruckTypeName","Type"),
                new GridColumn("TruckPlateNumber", "Plate"),
                new GridColumn("TrailerPlateNumber", "Trailer Plate"),
                new GridColumn("BrandModel","Brand"),
                new GridColumn("OwnerName","Owner"),
                new GridColumn("CarrierName", "Carrier"),
                new GridColumn("CountryName", "Country")
            };
        }

        [HttpGet]
        [Authorize("Transit.Trucks.Edit")]
        public IActionResult Create()
        {
            var vm = new TruckFormVM
            {
                IsActive = true,
                TPInsuranceExpiryDate = DateTime.Today.AddYears(1)
            };
            PopulateViewBags();
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", vm);
        }

        [HttpGet]
        [Authorize("Transit.Trucks.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _db.Trucks.FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
            if (entity == null) return NotFound();

            var vm = new TruckFormVM
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                TruckTypeId = entity.TruckTypeId,
                CarrierId = entity.CarrierId,
                OwnerId = entity.OwnerId,
                LastDriverId = entity.LastDriverId,
                TruckReferenceTitle = entity.TruckReferenceTitle,
                TruckPlateNumber = entity.TruckPlateNumber,
                TrailerPlateNumber = entity.TrailerPlateNumber,
                MaxPayloadKg = entity.MaxPayloadKg,
                MaxVolumeCBM = entity.MaxVolumeCBM,
                CountryId = entity.CountryId,
                FleetSmartCardNo = entity.FleetSmartCardNo,
                TPInsuranceNo = entity.TPInsuranceNo,
                TPInsuranceExpiryDate = entity.TPInsuranceExpiryDate,
                AxleCount = entity.AxleCount,
                Brand = entity.Brand,
                Model = entity.Model,
                YearManufactured = entity.YearManufactured,
                //IsActive = entity.IsActive
            };

            PopulateViewBags();
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.Trucks.Edit")]
        public async Task<IActionResult> Create(TruckFormVM vm)
        {
            await ValidateTruck(vm);
            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                ViewBag.CreateEdit = "Create";
                Response.StatusCode = 422;
                return PartialView("_CreateEdit", vm);
            }

            var entity = new Truck
            {
                Id = Guid.CreateVersion7(),
                CompanyId = vm.CompanyId,
                TruckTypeId = vm.TruckTypeId,
                CarrierId = vm.CarrierId,
                OwnerId = vm.OwnerId,
                LastDriverId = vm.LastDriverId,
                TruckReferenceTitle = vm.TruckReferenceTitle?.Trim(),
                TruckPlateNumber = vm.TruckPlateNumber?.Trim(),
                TrailerPlateNumber = vm.TrailerPlateNumber?.Trim(),
                MaxPayloadKg = vm.MaxPayloadKg,
                MaxVolumeCBM = vm.MaxVolumeCBM,
                CountryId = vm.CountryId,
                FleetSmartCardNo = vm.FleetSmartCardNo?.Trim(),
                TPInsuranceNo = vm.TPInsuranceNo?.Trim(),
                TPInsuranceExpiryDate = vm.TPInsuranceExpiryDate ?? DateTime.Now,
                AxleCount = vm.AxleCount,
                Brand = vm.Brand?.Trim(),
                Model = vm.Model?.Trim(),
                YearManufactured = vm.YearManufactured,
                //IsActive = vm.IsActive,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUser.UserId
            };

            _db.Trucks.Add(entity);
            await _db.SaveChangesAsync();

            HxTrigger(new { GridChanged = true, toastSuccess = "Truck created successfully" });
            return Content("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.Trucks.Edit")]
        public async Task<IActionResult> Edit(TruckFormVM vm)
        {
            await ValidateTruck(vm);
            if (!ModelState.IsValid)
            {
                PopulateViewBags();
                ViewBag.CreateEdit = "Edit";
                Response.StatusCode = 422;
                return PartialView("_CreateEdit", vm);
            }

            var entity = await _db.Trucks.FirstOrDefaultAsync(t => t.Id == vm.Id && !t.IsDeleted);
            if (entity == null) return NotFound();

            entity.CompanyId = vm.CompanyId;
            entity.TruckTypeId = vm.TruckTypeId;
            entity.CarrierId = vm.CarrierId;
            entity.OwnerId = vm.OwnerId;
            entity.LastDriverId = vm.LastDriverId;
            entity.TruckReferenceTitle = vm.TruckReferenceTitle?.Trim();
            entity.TruckPlateNumber = vm.TruckPlateNumber?.Trim();
            entity.TrailerPlateNumber = vm.TrailerPlateNumber?.Trim();
            entity.MaxPayloadKg = vm.MaxPayloadKg;
            entity.MaxVolumeCBM = vm.MaxVolumeCBM;
            entity.CountryId = vm.CountryId;
            entity.FleetSmartCardNo = vm.FleetSmartCardNo?.Trim();
            entity.TPInsuranceNo = vm.TPInsuranceNo?.Trim();
            entity.TPInsuranceExpiryDate = vm.TPInsuranceExpiryDate ?? DateTime.Now;
            entity.AxleCount = vm.AxleCount;
            entity.Brand = vm.Brand?.Trim();
            entity.Model = vm.Model?.Trim();
            entity.YearManufactured = vm.YearManufactured;
            //entity.IsActive = vm.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            HxTrigger(new { GridChanged = true, toastSuccess = "Truck updated successfully" });
            return Content("");
        }

        [HttpGet]
        [Authorize("Transit.Trucks.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var exists = await _db.Trucks.AnyAsync(x => x.Id == id && !x.IsDeleted);
            if (!exists) return NotFound();
            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Transit.Trucks.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var entity = await _db.Trucks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
            {
                HxTrigger(new { GridChanged = false, toastError = "Record not found" });
                return Content("");
            }

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUser.UserId;

            await _db.SaveChangesAsync();

            HxTrigger(new { GridChanged = true, toastSuccess = "Truck deleted successfully" });
            return Content("");
        }

        private async Task ValidateTruck(TruckFormVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.TruckPlateNumber)) return;

            var duplicatePlate = await _db.Trucks.AnyAsync(t =>
                !t.IsDeleted &&
                t.TruckPlateNumber.ToLower() == vm.TruckPlateNumber.Trim().ToLower() &&
                t.Id != (vm.Id ?? Guid.Empty));

            if (duplicatePlate)
                ModelState.AddModelError("TruckPlateNumber", "A truck with this plate number already exists.");
        }

        private void PopulateViewBags()
        {
            ViewBag.Companies = _db.Companies.Include(x => x.Person).OrderBy(c => c.Person.PersonName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Person.PersonName }).ToList();

            ViewBag.TruckTypes = _db.TruckTypes.OrderBy(t => t.TruckTypeName)
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.TruckTypeName }).ToList();

            ViewBag.Countries = _db.Countries.OrderBy(c => c.CountryName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CountryName }).ToList();

            ViewBag.Carriers = _db.Carriers.Where(c => !c.IsDeleted).OrderBy(c => c.Person.PersonName)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Person.PersonName }).ToList();

            ViewBag.Persons = _db.Persons.Where(p => !p.IsDeleted).OrderBy(p => p.PersonName)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.PersonName }).ToList();
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using ShipRate.Application.Interfaces;
//using ShipRate.Domain.Entities.Operations;
//using ShipRate.Infrastructure.Data;
//using ShipRate.WebApp.ViewModels.TransitViewModels;

//namespace ShipRate.WebApp.Controllers
//{
//    public class TruckController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly ICurrentUserService _currentUser;
//        private const int PageSize = 10;

//        public TruckController(ApplicationDbContext context, ICurrentUserService currentUser)
//        {
//            _db = context;
//            _currentUser = currentUser;
//        }

//        // amamamamam GET: /Truck
//        public IActionResult Index()
//        {
//            return View();
//        }

//        // amamamamam GET: /Truck/Grid
//        public async Task<IActionResult> Grid(string? search, int page = 1)
//        {
//            var query = _db.Trucks
//                .Include(t => t.TruckType)
//                .Include(t => t.Carrier)
//                .ThenInclude(t => t.Person)
//                .Include(t => t.Owner)
//                .Include(t => t.LastDriver)
//                .Include(t => t.Country)
//                .Include(t => t.Company)
//                .ThenInclude(t => t.Person)
//                .Where(t => !t.IsDeleted)
//                .AsQueryable();

//            // ---------- Search ----------
//            if (!string.IsNullOrWhiteSpace(search))
//            {
//                var term = search.Trim().ToLower();
//                query = query.Where(t =>
//                    t.TruckReferenceTitle.ToLower().Contains(term) ||
//                    t.TruckPlateNumber.ToLower().Contains(term) ||
//                    t.TrailerPlateNumber.ToLower().Contains(term) ||
//                    t.TruckType.TruckTypeName.ToLower().Contains(term) ||
//                    (t.Owner != null && (t.Owner.FirstName + " " + t.Owner.LastName).ToLower().Contains(term)) ||
//                    (t.Carrier != null && t.Carrier.Person.PersonName.ToLower().Contains(term)));
//            }

//            // ---------- Count & Pagination ----------
//            var totalCount = await query.CountAsync();
//            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

//            if (page < 1) page = 1;
//            if (page > totalPages && totalPages > 0) page = totalPages;

//            var items = await query
//                .OrderByDescending(t => t.CreatedAt)
//                .Skip((page - 1) * PageSize)
//                .Take(PageSize)
//                .Select(t => new TruckVM
//                {
//                    Id = t.Id,
//                    TruckReferenceTitle = t.TruckReferenceTitle,
//                    TruckTypeName = t.TruckType.TruckTypeName,
//                    TruckPlateNumber = t.TruckPlateNumber,
//                    TrailerPlateNumber = t.TrailerPlateNumber,
//                    OwnerName = t.Owner != null ? t.Owner.FirstName + " " + t.Owner.LastName : "—",
//                    CarrierName = t.Carrier != null ? t.Carrier.Person.PersonName : "—",
//                    LastDriverName = t.LastDriver != null ? t.LastDriver.FirstName + " " + t.LastDriver.LastName : "—",
//                    CountryName = t.Country.CountryName,
//                    BrandModel = t.Brand + " - " + t.Model,
//                    MaxPayloadKg = t.MaxPayloadKg,
//                    MaxVolumeCBM = t.MaxVolumeCBM,
//                    IsActive = t.IsActive
//                })
//                .ToListAsync();

//            ViewBag.CurrentPage = page;
//            ViewBag.TotalPages = totalPages;
//            ViewBag.TotalCount = totalCount;
//            ViewBag.Search = search;

//            return PartialView("_Grid", items);
//        }

//        // amamamamam GET: /Truck/Create
//        public IActionResult Create()
//        {
//            var vm = new TruckFormVM
//            {
//                IsActive = true,
//                TPInsuranceExpiryDate = DateTime.Today.AddYears(1)
//            };
//            PopulateViewBags();
//            return PartialView("_Form", vm);
//        }

//        // amamamamam GET: /Truck/Edit/{id}
//        public async Task<IActionResult> Edit(Guid id)
//        {
//            var entity = await _db.Trucks
//                .Where(t => !t.IsDeleted && t.Id == id)
//                .FirstOrDefaultAsync();

//            if (entity == null)
//                return NotFound();

//            var vm = new TruckFormVM
//            {
//                Id = entity.Id,
//                CompanyId = entity.CompanyId,
//                TruckTypeId = entity.TruckTypeId,
//                CarrierId = entity.CarrierId,
//                OwnerId = entity.OwnerId,
//                LastDriverId = entity.LastDriverId,
//                TruckReferenceTitle = entity.TruckReferenceTitle,
//                TruckPlateNumber = entity.TruckPlateNumber,
//                TrailerPlateNumber = entity.TrailerPlateNumber,
//                MaxPayloadKg = entity.MaxPayloadKg,
//                MaxVolumeCBM = entity.MaxVolumeCBM,
//                CountryId = entity.CountryId,
//                FleetSmartCardNo = entity.FleetSmartCardNo,
//                TPInsuranceNo = entity.TPInsuranceNo,
//                TPInsuranceExpiryDate = entity.TPInsuranceExpiryDate,
//                AxleCount = entity.AxleCount,
//                Brand = entity.Brand,
//                Model = entity.Model,
//                YearManufactured = entity.YearManufactured,
//                IsActive = entity.IsActive
//            };

//            PopulateViewBags();
//            return PartialView("_Form", vm);
//        }

//        // amamamamam POST: /Truck/Save
//        [HttpPost]
//        public async Task<IActionResult> Save(TruckFormVM vm)
//        {
//            // ========== Server-side Validation ==========
//            if (string.IsNullOrWhiteSpace(vm.TruckReferenceTitle))
//                ModelState.AddModelError("TruckReferenceTitle", "Reference title is required.");
//            else if (vm.TruckReferenceTitle.Trim().Length > 50)
//                ModelState.AddModelError("TruckReferenceTitle", "Reference title cannot exceed 50 characters.");

//            if (string.IsNullOrWhiteSpace(vm.TruckPlateNumber))
//                ModelState.AddModelError("TruckPlateNumber", "Truck plate number is required.");
//            else if (vm.TruckPlateNumber.Trim().Length > 30)
//                ModelState.AddModelError("TruckPlateNumber", "Truck plate number cannot exceed 30 characters.");

//            if (string.IsNullOrWhiteSpace(vm.TrailerPlateNumber))
//                ModelState.AddModelError("TrailerPlateNumber", "Trailer plate number is required.");
//            else if (vm.TrailerPlateNumber.Trim().Length > 30)
//                ModelState.AddModelError("TrailerPlateNumber", "Trailer plate number cannot exceed 30 characters.");

//            if (vm.CompanyId <= 0)
//                ModelState.AddModelError("CompanyId", "Please select a company.");

//            if (vm.TruckTypeId <= 0)
//                ModelState.AddModelError("TruckTypeId", "Please select a truck type.");

//            if (vm.CountryId <= 0)
//                ModelState.AddModelError("CountryId", "Please select a country.");

//            if (string.IsNullOrWhiteSpace(vm.Brand))
//                ModelState.AddModelError("Brand", "Brand is required.");

//            if (string.IsNullOrWhiteSpace(vm.Model))
//                ModelState.AddModelError("Model", "Model is required.");

//            if (string.IsNullOrWhiteSpace(vm.FleetSmartCardNo))
//                ModelState.AddModelError("FleetSmartCardNo", "Fleet smart card number is required.");

//            if (string.IsNullOrWhiteSpace(vm.TPInsuranceNo))
//                ModelState.AddModelError("TPInsuranceNo", "TP insurance number is required.");

//            if (vm.TPInsuranceExpiryDate == null)
//                ModelState.AddModelError("TPInsuranceExpiryDate", "Insurance expiry date is required.");

//            if (vm.MaxPayloadKg.HasValue && vm.MaxPayloadKg < 0)
//                ModelState.AddModelError("MaxPayloadKg", "Max payload cannot be negative.");

//            if (vm.MaxVolumeCBM.HasValue && vm.MaxVolumeCBM < 0)
//                ModelState.AddModelError("MaxVolumeCBM", "Max volume cannot be negative.");

//            if (vm.YearManufactured.HasValue && (vm.YearManufactured < 1950 || vm.YearManufactured > DateTime.Now.Year + 1))
//                ModelState.AddModelError("YearManufactured", $"Year must be between 1950 and {DateTime.Now.Year + 1}.");

//            if (vm.AxleCount.HasValue && (vm.AxleCount < 1 || vm.AxleCount > 20))
//                ModelState.AddModelError("AxleCount", "Axle count must be between 1 and 20.");

//            // ---- Duplicate plate check ----
//            if (!string.IsNullOrWhiteSpace(vm.TruckPlateNumber))
//            {
//                var duplicatePlate = await _db.Trucks.AnyAsync(t =>
//                    !t.IsDeleted &&
//                    t.TruckPlateNumber.ToLower() == vm.TruckPlateNumber.Trim().ToLower() &&
//                    t.Id != (vm.Id ?? Guid.Empty));

//                if (duplicatePlate)
//                    ModelState.AddModelError("TruckPlateNumber", "A truck with this plate number already exists.");
//            }

//            // ---- Duplicate trailer plate check ----
//            if (!string.IsNullOrWhiteSpace(vm.TrailerPlateNumber))
//            {
//                var duplicateTrailer = await _db.Trucks.AnyAsync(t =>
//                    !t.IsDeleted &&
//                    t.TrailerPlateNumber.ToLower() == vm.TrailerPlateNumber.Trim().ToLower() &&
//                    t.Id != (vm.Id ?? Guid.Empty));

//                if (duplicateTrailer)
//                    ModelState.AddModelError("TrailerPlateNumber", "A trailer with this plate number already exists.");
//            }

//            // ---- If validation failed ----
//            if (!ModelState.IsValid)
//            {
//                PopulateViewBags();
//                return PartialView("_Form", vm);
//            }

//            // ========== Save ==========
//            if (vm.Id == null || vm.Id == Guid.Empty)
//            {
//                // ---- CREATE ----
//                var entity = new Truck
//                {
//                    Id = Guid.CreateVersion7(),
//                    CompanyId = vm.CompanyId,
//                    TruckTypeId = vm.TruckTypeId,
//                    CarrierId = vm.CarrierId,
//                    OwnerId = vm.OwnerId,
//                    LastDriverId = vm.LastDriverId,
//                    TruckReferenceTitle = vm.TruckReferenceTitle.Trim(),
//                    TruckPlateNumber = vm.TruckPlateNumber.Trim(),
//                    TrailerPlateNumber = vm.TrailerPlateNumber.Trim(),
//                    MaxPayloadKg = vm.MaxPayloadKg,
//                    MaxVolumeCBM = vm.MaxVolumeCBM,
//                    CountryId = vm.CountryId,
//                    FleetSmartCardNo = vm.FleetSmartCardNo.Trim(),
//                    TPInsuranceNo = vm.TPInsuranceNo.Trim(),
//                    TPInsuranceExpiryDate = vm.TPInsuranceExpiryDate!.Value,
//                    AxleCount = vm.AxleCount,
//                    Brand = vm.Brand.Trim(),
//                    Model = vm.Model.Trim(),
//                    YearManufactured = vm.YearManufactured,
//                    IsActive = vm.IsActive,
//                    IsDeleted = false,
//                    CreatedAt = DateTime.Now,
//                    CreateUserId = _currentUser.UserId
//                };
//                _db.Trucks.Add(entity);
//            }
//            else
//            {
//                // ---- UPDATE ----
//                var entity = await _db.Trucks
//                    .FirstOrDefaultAsync(t => t.Id == vm.Id && !t.IsDeleted);

//                if (entity == null)
//                    return NotFound();

//                entity.CompanyId = vm.CompanyId;
//                entity.TruckTypeId = vm.TruckTypeId;
//                entity.CarrierId = vm.CarrierId;
//                entity.OwnerId = vm.OwnerId;
//                entity.LastDriverId = vm.LastDriverId;
//                entity.TruckReferenceTitle = vm.TruckReferenceTitle.Trim();
//                entity.TruckPlateNumber = vm.TruckPlateNumber.Trim();
//                entity.TrailerPlateNumber = vm.TrailerPlateNumber.Trim();
//                entity.MaxPayloadKg = vm.MaxPayloadKg;
//                entity.MaxVolumeCBM = vm.MaxVolumeCBM;
//                entity.CountryId = vm.CountryId;
//                entity.FleetSmartCardNo = vm.FleetSmartCardNo.Trim();
//                entity.TPInsuranceNo = vm.TPInsuranceNo.Trim();
//                entity.TPInsuranceExpiryDate = vm.TPInsuranceExpiryDate!.Value;
//                entity.AxleCount = vm.AxleCount;
//                entity.Brand = vm.Brand.Trim();
//                entity.Model = vm.Model.Trim();
//                entity.YearManufactured = vm.YearManufactured;
//                entity.IsActive = vm.IsActive;
//                entity.UpdatedAt = DateTime.Now;
//                entity.EditUserId = _currentUser.UserId;
//            }

//            await _db.SaveChangesAsync();

//            Response.Headers["HX-Trigger"] = "closeModalAndRefresh";
//            return Content("");
//        }

//        // amamamamam DELETE: /Truck/Delete/{id}
//        [HttpDelete]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var entity = await _db.Trucks
//                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

//            if (entity == null)
//                return NotFound();

//            entity.IsDeleted = true;
//            entity.UpdatedAt = DateTime.Now;
//            entity.EditUserId = _currentUser.UserId;

//            await _db.SaveChangesAsync();

//            return await Grid(null, 1);
//        }

//        // amamamamam Helper: Populate all dropdowns amamamamam
//        private void PopulateViewBags()
//        {
//            ViewBag.Companies = _db.Companies
//                .Include(x => x.Person)
//                .OrderBy(c => c.Person.PersonName)
//                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Person.PersonName })
//                .ToList();

//            ViewBag.TruckTypes = _db.TruckTypes
//                .OrderBy(t => t.TruckTypeName)
//                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.TruckTypeName })
//                .ToList();

//            ViewBag.Countries = _db.Countries
//                .OrderBy(c => c.CountryName)
//                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CountryName })
//                .ToList();

//            ViewBag.Carriers = _db.Carriers
//                .Where(c => !c.IsDeleted)
//                .OrderBy(c => c.Person.PersonName)
//                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Person.PersonName })
//                .ToList();

//            // Persons used for Owner and LastDriver
//            ViewBag.Persons = _db.Persons
//                .Where(p => !p.IsDeleted)
//                .OrderBy(p => p.FirstName).ThenBy(p => p.LastName)
//                .Select(p => new SelectListItem
//                {
//                    Value = p.Id.ToString(),
//                    Text = p.FirstName + " " + p.LastName
//                })
//                .ToList();
//        }
//    }
//}
