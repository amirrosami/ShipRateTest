using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.TransitViewModels;
using System.Linq.Dynamic.Core;
using System.Text.Json;
namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private const int PageSize = 10;

        public TripsController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }


        [Authorize("Transit.Trips.View")]
        public async Task<IActionResult> Index()
        {
            await LoadDropdowns();
            var trips = await getFilteredList(new PagedRequestDto());
            return View(trips);
        }


        private async Task<ViewModels.PagedResult<TripItemViewModel>> getFilteredList(PagedRequestDto dto)
        {
            var data = _db.Trips
                 .Include(t => t.Truck)
                 .Include(t => t.Driver).ThenInclude(d => d.Person)
                 .Include(t => t.FromLocation)
                 .Include(t => t.ToLocation)
                 .Include(t => t.TripStatus)
                 .Include(t => t.Carrier).ThenInclude(c => c.Person)
                 .Where(t => !t.IsDeleted);


            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.TripNumber.Contains(dto.SearchTerm) || x.TripStatus.TripStatusName.Contains(dto.SearchTerm) || x.Driver.Person.PersonName.Contains(dto.SearchTerm) || x.Carrier.Person.PersonName.Contains(dto.SearchTerm));
            }

            var select = data.Select(t => new TripItemViewModel
            {
                Id = t.Id,
                TripNumber = t.TripNumber,
                TruckPlateNumber = t.Truck.TruckPlateNumber,
                TruckTitle = t.Truck.TruckReferenceTitle,
                DriverName = t.Driver.Person.PersonName,
                FromLocation = t.FromLocation.LocationName,
                ToLocation = t.ToLocation.LocationName,
                DepartureDate = t.DepartureDate,
                ArrivalDate = t.ArrivalDate,
                StatusName = t.TripStatus.TripStatusName,
                TripStatusId = t.TripStatusId,
                CarrierName = t.Carrier.Person.PersonName,
                //IsActive = t.IsActive,
                ETA = t.ETA,
                ETD = t.ETD,
                Route = $"{t.FromLocation.LocationName} -> {t.ToLocation.LocationName}"
            });

            int total = await select.CountAsync();

            if (!string.IsNullOrEmpty(dto.SortColumn) && !string.IsNullOrEmpty(dto.SortDirection))
            {
                select = select.OrderBy($"{dto.SortColumn}  {dto.SortDirection}");
            }
            var items = await select.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync();


            return new ViewModels.PagedResult<TripItemViewModel>()
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
        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);

            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Trips/List",
                GridPartialName = "_List",
                columns = GetGridColumns()
            };

            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
            new GridColumn("TripNumber","Trip Number"),
            new GridColumn("TruckPlateNumber","Truck Plate"),
            new GridColumn("DriverName","Driver"),
            new GridColumn("Route","Route"),
            new GridColumn("ETD","ETD"),
            new GridColumn("ETA","ETA"),
            new GridColumn("DepartureDate","Departure Date"),
            new GridColumn("ArrivalDate","Arrival Date"),
            new GridColumn("CarrierName","Carrier"),
            new GridColumn("StatusName","Status")
            };
        }

        [HttpGet]
        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> Create()
        {
            await LoadFormDropdowns();
            ViewBag.CreateEdit = "Create";
            return PartialView("_TripForm", new TripViewModel());
        }

        [HttpPost]
        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> Create(TripViewModel model)
        {
            ModelState.Clear();
            var trip = new Trip
            {
                Id = Guid.CreateVersion7(),
                CompanyId = model.CompanyId,
                TruckId = model.TruckId,
                TripNumber = model.TripNumber,
                DriverId = model.DriverId,
                FromLocationId = model.FromLocationId,
                ToLocationId = model.ToLocationId,
                DepartureDate = model.DepartureDate,
                ArrivalDate = model.ArrivalDate,
                TripStatusId = 1,  //Registered
                CarrierId = model.CarrierId,
                Remarks = model.Remarks ?? "",
                ETA = model.ETA,
                ETD = model.ETD,
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = GetCurrentUserId()
            };
            var validation = new TripValidation().Validate(trip);
            if (!validation.IsValid)
            {
                foreach (var item in validation.Errors)
                {
                    ModelState.AddModelError("", item.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.CreateEdit = "Create";
                await LoadFormDropdowns();
                return PartialView("_TripForm", model);
            }
            _db.Trips.Add(trip);
            try
            {
                await _db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw e;
            }
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Content("");
        }

        [HttpGet]
        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var trip = await _db.Trips.Include(x => x.FromLocation).Include(x => x.ToLocation).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (trip == null) return NotFound();

            await LoadFormDropdowns();
            var model = new TripViewModel
            {
                Id = trip.Id,
                CompanyId = trip.CompanyId,
                TruckId = trip.TruckId,
                TripNumber = trip.TripNumber,
                DriverId = trip.DriverId,
                FromLocationId = trip.FromLocationId,
                ToLocationId = trip.ToLocationId,
                FromLocationName = trip.FromLocation.LocationName,
                ToLocationName = trip.ToLocation.LocationName,
                DepartureDate = trip.DepartureDate,
                ArrivalDate = trip.ArrivalDate,
                TripStatusId = trip.TripStatusId,
                CarrierId = trip.CarrierId,
                Remarks = trip.Remarks,
                ETD = trip.ETD,
                ETA = trip.ETA,
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_TripForm", model);
        }
        [HttpPost]
        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> Edit(Guid id, TripViewModel model)
        {
            ModelState.Clear();
            var trip = await _db.Trips.FindAsync(id);
            if (trip == null) return NotFound();

            trip.CompanyId = model.CompanyId;
            trip.TruckId = model.TruckId;
            trip.TripNumber = model.TripNumber;
            trip.DriverId = model.DriverId;
            trip.FromLocationId = model.FromLocationId;
            trip.ToLocationId = model.ToLocationId;
            trip.DepartureDate = model.DepartureDate;
            trip.ArrivalDate = model.ArrivalDate;
            trip.TripStatusId = model.TripStatusId;
            trip.CarrierId = model.CarrierId;
            trip.Remarks = model.Remarks ?? "";
            trip.UpdatedAt = DateTime.Now;
            trip.EditUserId = GetCurrentUserId();
            trip.ETA = model.ETA;
            trip.ETD = model.ETD;
            var validation = new TripValidation().Validate(trip);
            if (!validation.IsValid)
            {
                foreach (var item in validation.Errors)
                {
                    ModelState.AddModelError("", item.ErrorMessage);
                }
            }
            if (!ModelState.IsValid)
            {
                await LoadFormDropdowns();
                ViewBag.CreateEdit = "Edit";
                return PartialView("_TripForm", model);
            }
            await _db.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }

        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        [Authorize("Transit.Trips.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var trip = await _db.Trips.FindAsync(id);
            if (trip == null) return NotFound();

            trip.IsDeleted = true;
            trip.DeletedAt = DateTime.Now;
            trip.DeleteUserId = GetCurrentUserId();
            await _db.SaveChangesAsync();
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Deleted Sussefully"
            });
            return Content("");
        }

        // ---- Private Helpers ----

        private async Task<TripListViewModel> GetFilteredTrips(
            string? plateNumber, string? driverName, int? statusId,
            Guid? fromLocationId, Guid? toLocationId,
            DateTime? dateFrom, DateTime? dateTo, int page)
        {
            var query = _db.Trips
                .Include(t => t.Truck)
                .Include(t => t.Driver).ThenInclude(d => d.Person)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .Include(t => t.TripStatus)
                .Include(t => t.Carrier).ThenInclude(c => c.Person)
                .Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(plateNumber))
                query = query.Where(t => t.Truck.TruckPlateNumber.Contains(plateNumber));

            if (!string.IsNullOrWhiteSpace(driverName))
                query = query.Where(t => t.Driver.Person.PersonName.Contains(driverName));

            if (statusId.HasValue && statusId > 0)
                query = query.Where(t => t.TripStatusId == statusId);

            if (fromLocationId.HasValue && fromLocationId != Guid.Empty)
                query = query.Where(t => t.FromLocationId == fromLocationId);

            if (toLocationId.HasValue && toLocationId != Guid.Empty)
                query = query.Where(t => t.ToLocationId == toLocationId);

            if (dateFrom.HasValue)
                query = query.Where(t => t.DepartureDate >= dateFrom);

            if (dateTo.HasValue)
                query = query.Where(t => t.ArrivalDate <= dateTo);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(t => new TripItemViewModel
                {
                    Id = t.Id,
                    TripNumber = t.TripNumber,
                    TruckPlateNumber = t.Truck.TruckPlateNumber,
                    TruckTitle = t.Truck.TruckReferenceTitle,
                    DriverName = t.Driver.Person.PersonName,
                    FromLocation = t.FromLocation.LocationName,
                    ToLocation = t.ToLocation.LocationName,
                    DepartureDate = t.DepartureDate,
                    ArrivalDate = t.ArrivalDate,
                    StatusName = t.TripStatus.TripStatusName,
                    TripStatusId = t.TripStatusId,
                    CarrierName = t.Carrier.Person.PersonName,
                    //IsActive = t.IsActive,
                    ETD = t.ETD,
                    ETA = t.ETA,
                })
                .ToListAsync();

            return new TripListViewModel
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PlateNumber = plateNumber,
                DriverName = driverName,
                StatusId = statusId,
                FromLocationId = fromLocationId,
                ToLocationId = toLocationId,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
        }

        private async Task LoadDropdowns()
        {
            ViewBag.Statuses = await _db.TripStatuses.ToListAsync();
        }
        private async Task LoadFormDropdowns()
        {
            await LoadDropdowns();
            ViewBag.Trucks = await _db.Trucks.Where(t => !t.IsDeleted).ToListAsync();
            ViewBag.Drivers = await _db.Drivers.Include(d => d.Person).Where(d => !d.IsDeleted).ToListAsync();
            ViewBag.Carriers = await _db.Carriers.Include(c => c.Person).Where(c => !c.IsDeleted && (c.CarrierTypeId == 4 || c.CarrierTypeId == 5)).ToListAsync();
            ViewBag.Companies = await _db.Companies.Include(x => x.Person).Where(c => !c.IsDeleted).ToListAsync();
        }
        private Guid GetCurrentUserId() =>
            _currentUserService.UserId;
    }
}
