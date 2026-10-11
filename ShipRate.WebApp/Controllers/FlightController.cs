using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.FlightVms;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly int _companyId = 1; // from session/claims

        public FlightController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        private IActionResult HtmxSuccess(string triggerEvent)
        {
            Response.Headers.Add("HX-Trigger", triggerEvent);
            return Ok();
        }


        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");

        private void HxTrigger(object triggers)
        {
            Response.Headers.Append("HX-Trigger", JsonSerializer.Serialize(triggers));
        }

        private async Task<PagedResult<FlightListItem>> getFilteredList(PagedRequestDto dto)
        {
            var query = _db.Flights
                .Where(f => !f.IsDeleted && f.CompanyId == _companyId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                query = query.Where(f =>
                    f.FlightNumber.Contains(dto.SearchTerm) ||
                    _db.Carriers.Include(x => x.Person)
                        .Where(c => c.Id == f.AirLineId)
                        .Select(c => c.Person.PersonName)
                        .FirstOrDefault()!.Contains(dto.SearchTerm) ||
                    _db.Locations.Where(l => l.Id == f.OriginAirportId)
                        .Select(l => l.LocationName)
                        .FirstOrDefault()!.Contains(dto.SearchTerm) ||
                    _db.Locations.Where(l => l.Id == f.DestinationAirportId)
                        .Select(l => l.LocationName)
                        .FirstOrDefault()!.Contains(dto.SearchTerm)
                );
            }

            int total = await query.CountAsync();

            query = query
                .OrderBy(f => f.FlightNumber)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize);

            var items = await query
                .Select(f => new FlightListItem
                {
                    Id = f.Id,
                    FlightNumber = f.FlightNumber,
                    AirLineName = _db.Carriers
                        .Include(x => x.Person)
                        .Where(c => c.Id == f.AirLineId)
                        .Select(c => c.Person.PersonName)
                        .FirstOrDefault(),
                    OriginAirportName = _db.Locations
                        .Where(l => l.Id == f.OriginAirportId)
                        .Select(l => l.LocationName)
                        .FirstOrDefault(),
                    DestinationAirportName = _db.Locations
                        .Where(l => l.Id == f.DestinationAirportId)
                        .Select(l => l.LocationName)
                        .FirstOrDefault(),
                    //IsActive = f.IsActive
                })
                .ToListAsync();

            return new PagedResult<FlightListItem>
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection,
                TotalCount = total
            };
        }

        [HttpGet]
        [Authorize("BaseOperations.Flights.View")]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }

        [HttpPost]
        [Authorize("BaseOperations.Flights.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);

            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Flight/List",
                GridPartialName = "_List",
                columns = GetGridColumns()
            };

            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn>
            {
                new GridColumn { DisplayName = "Flight Number", Name = "FlightNumber" },
                new GridColumn { DisplayName = "Air Line", Name = "AirLineName" },
                new GridColumn("OriginAirportName", "Origin Airport"),
                new GridColumn("DestinationAirportName", "Destination Airport")
            };
        }

        [HttpGet]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> Create()
        {
            ViewBag.AirLines = new SelectList(await GetAirLinesDropdown(), "Value", "Text");
            ViewBag.Airports = new SelectList(await GetAirportsDropdown(), "Value", "Text");
            ViewBag.CreateEdit = "Create";
            return PartialView("_FlightForm", new FlightFormViewModel
            {
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> Create(FlightFormViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AirLines = new SelectList(await GetAirLinesDropdown(), "Value", "Text");
                ViewBag.Airports = new SelectList(await GetAirportsDropdown(), "Value", "Text");
                Response.StatusCode = 422;
                ViewBag.CreateEdit = "Create";
                return PartialView("_FlightForm", dto);
            }

            var flight = new Domain.Entities.Operations.Flight
            {
                Id = Guid.CreateVersion7(),
                CompanyId = _companyId,
                FlightNumber = dto.FlightNumber,
                AirLineId = dto.AirLineId,
                OriginAirportId = dto.OriginAirportId,
                DestinationAirportId = dto.DestinationAirportId,
                //IsActive = dto.IsActive,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };

            _db.Flights.Add(flight);
            await _db.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });

            return Content("");
        }

        [HttpGet]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var flight = await _db.Flights.FindAsync(id);
            if (flight == null || flight.IsDeleted)
                return NotFound();

            ViewBag.AirLines = new SelectList(await GetAirLinesDropdown(), "Value", "Text");
            ViewBag.Airports = new SelectList(await GetAirportsDropdown(), "Value", "Text");
            ViewBag.CreateEdit = "Edit";

            var dto = new FlightFormViewModel
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                AirLineId = flight.AirLineId,
                OriginAirportId = flight.OriginAirportId,
                DestinationAirportId = flight.DestinationAirportId,
                //IsActive = flight.IsActive
            };

            return PartialView("_FlightForm", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> Edit(FlightFormViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AirLines = new SelectList(await GetAirLinesDropdown(), "Value", "Text");
                ViewBag.Airports = new SelectList(await GetAirportsDropdown(), "Value", "Text");
                ViewBag.CreateEdit = "Edit";
                Response.StatusCode = 422;
                return PartialView("_FlightForm", dto);
            }

            var flight = await _db.Flights.FindAsync(dto.Id);
            if (flight == null || flight.IsDeleted)
                return NotFound();

            flight.FlightNumber = dto.FlightNumber;
            flight.AirLineId = dto.AirLineId;
            flight.OriginAirportId = dto.OriginAirportId;
            flight.DestinationAirportId = dto.DestinationAirportId;
            //flight.IsActive = dto.IsActive;
            flight.UpdatedAt = DateTime.Now;
            flight.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });

            return Content("");
        }

        [HttpGet]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return PartialView("_DeleteConfirm", id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var flight = await _db.Flights.FindAsync(id);
            if (flight == null || flight.IsDeleted)
            {
                HxTrigger(new
                {
                    gridChanged = false,
                    toastError = "Record not found"
                });
                return Content("");
            }

            flight.IsDeleted = true;
            flight.UpdatedAt = DateTime.Now;
            flight.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Deleted Sussefully"
            });

            return Content("");
        }

        private async Task<List<SelectListItem>> GetAirLinesDropdown()
        {
            return await _db.Carriers
                .Include(x => x.Person)
                .Where(c => !c.IsDeleted && c.CarrierTypeId == 3)
                .OrderBy(c => c.Person.PersonName)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Person.PersonName
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetAirportsDropdown()
        {
            return await _db.Locations
                .Where(l => l.IsActive && !l.IsDeleted && l.LocationTypeId == 3)
                .OrderBy(l => l.LocationName)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.LocationName
                })
                .ToListAsync();
        }

        [Authorize("BaseOperations.Flights.View")]
        public async Task<IActionResult> LoadFlightDateGrid(Guid flightId)
        {
            var flightDates = await _db.FlightDates
                .Where(fd => fd.FlightId == flightId && !fd.IsDeleted)
                .OrderBy(fd => fd.ETD)
                .Select(fd => new FlightDateListItem
                {
                    Id = fd.Id,
                    FlightId = fd.FlightId,
                    ETD = fd.ETD,
                    ETA = fd.ETA,
                    ActualDeparture = fd.ActualDeparture,
                    ActualArrival = fd.ActualArrival,
                    DelayHours = fd.DelayHours,
                    Description = fd.Description,
                    //IsActive = fd.IsActive
                })
                .ToListAsync();

            ViewBag.FlightId = flightId;
            return PartialView("_FlightDateGrid", flightDates);
        }

        [HttpGet]
        [Authorize("BaseOperations.Flights.Edit")]
        public IActionResult CreateFlightDate(Guid flightId)
        {
            var viewModel = new FlightDateFormViewModel
            {
                Id = Guid.Empty,
                FlightId = flightId,
            };

            return PartialView("_FlightDateForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Flights.Edit")]

        public async Task<IActionResult> CreateFlightDate(FlightDateFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_FlightDateForm", model);







            var flightDate = new FlightDate
            {
                Id = Guid.CreateVersion7(),
                FlightId = model.FlightId,
                ETD = model.ETD,
                ETA = model.ETA,
                ActualDeparture = model.ActualDeparture,
                ActualArrival = model.ActualArrival,
                DelayHours = model.DelayHours,
                Description = model.Description,
                //IsActive = model.IsActive,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };

            _db.FlightDates.Add(flightDate);
            await _db.SaveChangesAsync();

            return HtmxSuccess("flightDateChanged");
        }

        [HttpGet]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> EditFlightDate(Guid id)
        {
            var flightDate = await _db.FlightDates.FindAsync(id);
            if (flightDate == null || flightDate.IsDeleted)
                return NotFound();

            var viewModel = new FlightDateFormViewModel
            {
                Id = flightDate.Id,
                FlightId = flightDate.FlightId,
                ETD = flightDate.ETD,
                ETA = flightDate.ETA,
                ActualDeparture = flightDate.ActualDeparture,
                ActualArrival = flightDate.ActualArrival,
                DelayHours = flightDate.DelayHours,
                Description = flightDate.Description,
                //IsActive = flightDate.IsActive,
            };

            return PartialView("_FlightDateForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> EditFlightDate(FlightDateFormViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_FlightDateForm", model);

            var flightDate = await _db.FlightDates.FindAsync(model.Id);
            if (flightDate == null || flightDate.IsDeleted)
                return NotFound();

            flightDate.ETD = model.ETD;
            flightDate.ETA = model.ETA;
            flightDate.ActualDeparture = model.ActualDeparture;
            flightDate.ActualArrival = model.ActualArrival;
            flightDate.DelayHours = model.DelayHours;
            flightDate.Description = model.Description;
            //flightDate.IsActive = model.IsActive;
            flightDate.UpdatedAt = DateTime.Now;
            flightDate.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();

            return HtmxSuccess("flightDateChanged");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Flights.Edit")]
        public async Task<IActionResult> DeleteFlightDate(Guid id)
        {
            var flightDate = await _db.FlightDates.FindAsync(id);
            if (flightDate == null || flightDate.IsDeleted)
                return NotFound();

            flightDate.IsDeleted = true;
            flightDate.UpdatedAt = DateTime.Now;
            flightDate.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();

            return HtmxSuccess("flightDateChanged");
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using ShipRate.Application.Interfaces;
//using ShipRate.Domain.Entities.Operations;
//using ShipRate.Infrastructure.Data;
//using ShipRate.WebApp.ViewModels.FlightVms;

//namespace ShipRate.WebApp.Controllers
//{
//    public class FlightController : Controller
//    {
//        private readonly ApplicationDbContext _db;
//        private readonly ICurrentUserService _currentUserService;
//        private readonly int _companyId = 1; // از session/claims بگیرید


//        public FlightController(ApplicationDbContext db, ICurrentUserService currentUserService)
//        {
//            _db = db;
//            _currentUserService = currentUserService;
//        }

//        // ═══════════════════════════════════════════════════════════
//        // FLIGHTS - Main Grid
//        // ═══════════════════════════════════════════════════════════

//        public async Task<IActionResult> Index(string searchFlightNumber, Guid? searchAirLineId, int page = 1)
//        {
//            var viewModel = new FlightIndexViewModel
//            {
//                SearchFlightNumber = searchFlightNumber,
//                SearchAirLineId = searchAirLineId,
//                CurrentPage = page,
//                AirLines = await GetAirLinesDropdown()
//            };

//            return View(viewModel);
//        }

//        public async Task<IActionResult> LoadFlightGrid(string searchFlightNumber, Guid? searchAirLineId, int page = 1, int pageSize = 10)
//        {
//            var query = _db.Flights
//                .Where(f => !f.IsDeleted && f.CompanyId == _companyId)
//                .AsQueryable();

//            if (!string.IsNullOrWhiteSpace(searchFlightNumber))
//                query = query.Where(f => f.FlightNumber.Contains(searchFlightNumber));

//            if (searchAirLineId.HasValue && searchAirLineId.Value != Guid.Empty)
//                query = query.Where(f => f.AirLineId == searchAirLineId.Value);

//            var totalCount = await query.CountAsync();
//            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

//            var flights = await query
//                .OrderBy(f => f.FlightNumber)
//                .Skip((page - 1) * pageSize)
//                .Take(pageSize)
//                .Select(f => new FlightListItem
//                {
//                    Id = f.Id,
//                    FlightNumber = f.FlightNumber,
//                    AirLineName = _db.Carriers.Include(x => x.Person).Where(c => c.Id == f.AirLineId).Select(c => c.Person.PersonName).FirstOrDefault(),
//                    OriginAirportName = _db.Locations.Where(l => l.Id == f.OriginAirportId).Select(l => l.LocationName).FirstOrDefault(),
//                    DestinationAirportName = _db.Locations.Where(l => l.Id == f.DestinationAirportId).Select(l => l.LocationName).FirstOrDefault(),
//                    IsActive = f.IsActive
//                })
//                .ToListAsync();

//            var viewModel = new FlightIndexViewModel
//            {
//                Flights = flights,
//                CurrentPage = page,
//                TotalPages = totalPages,
//                PageSize = pageSize,
//                SearchFlightNumber = searchFlightNumber,
//                SearchAirLineId = searchAirLineId
//            };

//            return PartialView("_FlightGrid", viewModel);
//        }

//        [HttpGet]
//        public async Task<IActionResult> CreateFlight()
//        {
//            var viewModel = new FlightFormViewModel
//            {
//                Id = Guid.Empty,
//                AirLines = await GetAirLinesDropdown(),
//                Airports = await GetAirportsDropdown()
//            };

//            return PartialView("_FlightForm", viewModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CreateFlight(FlightFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                model.AirLines = await GetAirLinesDropdown();
//                model.Airports = await GetAirportsDropdown();
//                return PartialView("_FlightForm", model);
//            }

//            var flight = new Flight
//            {
//                Id = Guid.CreateVersion7(),
//                CompanyId = _companyId,
//                FlightNumber = model.FlightNumber,
//                AirLineId = model.AirLineId,
//                OriginAirportId = model.OriginAirportId,
//                DestinationAirportId = model.DestinationAirportId,
//                IsActive = model.IsActive,
//                CreatedAt = DateTime.Now,
//                CreateUserId = _currentUserService.UserId,
//            };

//            _db.Flights.Add(flight);
//            await _db.SaveChangesAsync();

//            return HtmxSuccess("flightChanged");
//        }

//        [HttpGet]
//        public async Task<IActionResult> EditFlight(Guid id)
//        {
//            var flight = await _db.Flights.FindAsync(id);
//            if (flight == null || flight.IsDeleted)
//                return NotFound();

//            var viewModel = new FlightFormViewModel
//            {
//                Id = flight.Id,
//                FlightNumber = flight.FlightNumber,
//                AirLineId = flight.AirLineId,
//                OriginAirportId = flight.OriginAirportId,
//                DestinationAirportId = flight.DestinationAirportId,
//                IsActive = flight.IsActive,
//                AirLines = await GetAirLinesDropdown(),
//                Airports = await GetAirportsDropdown()
//            };

//            return PartialView("_FlightForm", viewModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> EditFlight(FlightFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                model.AirLines = await GetAirLinesDropdown();
//                model.Airports = await GetAirportsDropdown();
//                return PartialView("_FlightForm", model);
//            }

//            var flight = await _db.Flights.FindAsync(model.Id);
//            if (flight == null || flight.IsDeleted)
//                return NotFound();

//            flight.FlightNumber = model.FlightNumber;
//            flight.AirLineId = model.AirLineId;
//            flight.OriginAirportId = model.OriginAirportId;
//            flight.DestinationAirportId = model.DestinationAirportId;
//            flight.IsActive = model.IsActive;
//            flight.UpdatedAt = DateTime.Now;
//            flight.EditUserId = _currentUserService.UserId;

//            await _db.SaveChangesAsync();

//            return HtmxSuccess("flightChanged");
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteFlight(Guid id)
//        {
//            var flight = await _db.Flights.FindAsync(id);
//            if (flight == null || flight.IsDeleted)
//                return NotFound();

//            flight.IsDeleted = true;
//            flight.UpdatedAt = DateTime.Now;
//            flight.EditUserId = _currentUserService.UserId;

//            await _db.SaveChangesAsync();

//            return HtmxSuccess("flightChanged");
//        }

//        // ═══════════════════════════════════════════════════════════
//        // FLIGHT DATES - Child Grid
//        // ═══════════════════════════════════════════════════════════

//        public async Task<IActionResult> LoadFlightDateGrid(Guid flightId)
//        {
//            var flightDates = await _db.FlightDates
//                .Where(fd => fd.FlightId == flightId && !fd.IsDeleted)
//                .OrderBy(fd => fd.ETD)
//                .Select(fd => new FlightDateListItem
//                {
//                    Id = fd.Id,
//                    FlightId = fd.FlightId,
//                    ETD = fd.ETD,
//                    ETA = fd.ETA,
//                    ActualDeparture = fd.ActualDeparture,
//                    ActualArrival = fd.ActualArrival,
//                    DelayHours = fd.DelayHours,
//                    Description = fd.Description,
//                    IsActive = fd.IsActive
//                })
//                .ToListAsync();

//            ViewBag.FlightId = flightId;
//            return PartialView("_FlightDateGrid", flightDates);
//        }

//        [HttpGet]
//        public IActionResult CreateFlightDate(Guid flightId)
//        {
//            var viewModel = new FlightDateFormViewModel
//            {
//                Id = Guid.Empty,
//                FlightId = flightId,
//                TransportMovementId = Guid.Empty
//            };

//            return PartialView("_FlightDateForm", viewModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> CreateFlightDate(FlightDateFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return PartialView("_FlightDateForm", model);


//            var transportMovement = new TransportMovement
//            {
//                Id = Guid.CreateVersion7(),
//                CarrierId = _db.Flights.Where(f => f.Id == model.FlightId).FirstOrDefault().AirLineId,
//                MovementNumber = _db.Flights.Where(f => f.Id == model.FlightId).FirstOrDefault().FlightNumber + "_" + String.Format(model.ETD.ToString(), "yyyyMMdd"),
//                CompanyId = _companyId,
//                TransportTypeId = 4,
//                CreatedAt = DateTime.Now,
//                CreateUserId = _currentUserService.UserId
//            };





//            var flightDate = new FlightDate
//            {
//                Id = transportMovement.Id,
//                FlightId = model.FlightId,
//                ETD = model.ETD,
//                ETA = model.ETA,
//                ActualDeparture = model.ActualDeparture,
//                ActualArrival = model.ActualArrival,
//                DelayHours = model.DelayHours,
//                Description = model.Description,
//                IsActive = model.IsActive,
//                TransportMovementId = model.TransportMovementId == Guid.Empty ? Guid.Empty : model.TransportMovementId,
//                CreatedAt = DateTime.Now,
//                CreateUserId = _currentUserService.UserId
//            };

//            _db.TransportMovements.Add(transportMovement);
//            _db.FlightDates.Add(flightDate);
//            await _db.SaveChangesAsync();

//            return HtmxSuccess("flightDateChanged");
//        }

//        [HttpGet]
//        public async Task<IActionResult> EditFlightDate(Guid id)
//        {
//            var flightDate = await _db.FlightDates.FindAsync(id);
//            if (flightDate == null || flightDate.IsDeleted)
//                return NotFound();

//            var viewModel = new FlightDateFormViewModel
//            {
//                Id = flightDate.Id,
//                FlightId = flightDate.FlightId,
//                ETD = flightDate.ETD,
//                ETA = flightDate.ETA,
//                ActualDeparture = flightDate.ActualDeparture,
//                ActualArrival = flightDate.ActualArrival,
//                DelayHours = flightDate.DelayHours,
//                Description = flightDate.Description,
//                IsActive = flightDate.IsActive,
//                TransportMovementId = flightDate.TransportMovementId
//            };

//            return PartialView("_FlightDateForm", viewModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> EditFlightDate(FlightDateFormViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return PartialView("_FlightDateForm", model);

//            var flightDate = await _db.FlightDates.FindAsync(model.Id);
//            if (flightDate == null || flightDate.IsDeleted)
//                return NotFound();

//            flightDate.ETD = model.ETD;
//            flightDate.ETA = model.ETA;
//            flightDate.ActualDeparture = model.ActualDeparture;
//            flightDate.ActualArrival = model.ActualArrival;
//            flightDate.DelayHours = model.DelayHours;
//            flightDate.Description = model.Description;
//            flightDate.IsActive = model.IsActive;
//            flightDate.TransportMovementId = model.TransportMovementId;
//            flightDate.UpdatedAt = DateTime.Now;
//            flightDate.EditUserId = _currentUserService.UserId;

//            await _db.SaveChangesAsync();

//            return HtmxSuccess("flightDateChanged");
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteFlightDate(Guid id)
//        {
//            var flightDate = await _db.FlightDates.FindAsync(id);
//            if (flightDate == null || flightDate.IsDeleted)
//                return NotFound();

//            flightDate.IsDeleted = true;
//            flightDate.UpdatedAt = DateTime.Now;
//            flightDate.EditUserId = _currentUserService.UserId;

//            await _db.SaveChangesAsync();

//            return HtmxSuccess("flightDateChanged");
//        }

//        // ═══════════════════════════════════════════════════════════
//        // HELPER METHODS
//        // ═══════════════════════════════════════════════════════════

//        private async Task<List<SelectListItem>> GetAirLinesDropdown()
//        {
//            return await _db.Carriers
//                .Include(x => x.Person)
//                .Where(c => c.IsActive && !c.IsDeleted && c.CarrierTypeId == 3)
//                .OrderBy(c => c.Person.PersonName)
//                .Select(c => new SelectListItem
//                {
//                    Value = c.Id.ToString(),
//                    Text = c.Person.PersonName
//                })
//                .ToListAsync();
//        }

//        private async Task<List<SelectListItem>> GetAirportsDropdown()
//        {
//            return await _db.Locations
//                .Where(l => l.IsActive && !l.IsDeleted && l.LocationTypeId == 3)
//                .OrderBy(l => l.LocationName)
//                .Select(l => new SelectListItem
//                {
//                    Value = l.Id.ToString(),
//                    Text = l.LocationName
//                })
//                .ToListAsync();
//        }

//        private IActionResult HtmxSuccess(string triggerEvent)
//        {
//            Response.Headers.Add("HX-Trigger", triggerEvent);
//            return Ok();
//        }
//    }
//}
