using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.TransitViewModels;

namespace ShipRate.WebApp.Controllers
{
    // Controllers/TripStopsController.cs
    [Authorize]

    public class TripStopsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripStopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Load stops list for a trip
        [HttpGet]
        public async Task<IActionResult> Index(Guid tripId)
        {
            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null) return NotFound();

            var stops = await _context.TripStops
                .Include(s => s.Location)
                .Where(s => s.TripId == tripId && !s.IsDeleted)
                .OrderBy(s => s.StopNumber)
                .Select(s => new TripStopViewModel
                {
                    Id = s.Id,
                    TripId = s.TripId,
                    StopNumber = s.StopNumber,
                    LocationId = s.LocationId,
                    LocationName = s.Location.LocationName,
                    ScheduledArrival = s.ScheduledArrival,
                    ScheduledDeparture = s.ScheduledDeparture,
                    ActualArrival = s.ActualArrival,
                    ActualDeparture = s.ActualDeparture,
                    Remarks = s.Remarks
                })
                .ToListAsync();

            var model = new TripStopsListViewModel
            {
                TripId = tripId,
                TripNumber = trip.TripNumber,
                Stops = stops
            };

            return PartialView("_TripStopsModal", model);
        }

        // GET: Create form
        [HttpGet]
        public async Task<IActionResult> Create(Guid tripId)
        {
            var nextStopNumber = await _context.TripStops
                .Where(s => s.TripId == tripId && !s.IsDeleted)
                .Select(s => s.StopNumber)
                .DefaultIfEmpty(0)
                .MaxAsync() + 1;

            var model = new TripStopViewModel
            {
                TripId = tripId,
                StopNumber = nextStopNumber,
                ScheduledArrival = DateTime.Now,
                ScheduledDeparture = DateTime.Now.AddHours(2)
            };

            await LoadLocations();
            return PartialView("_TripStopForm", model);
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(TripStopViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLocations();
                return PartialView("_TripStopForm", model);
            }

            var stop = new TripStop
            {
                Id = Guid.CreateVersion7(),
                TripId = model.TripId,
                StopNumber = model.StopNumber,
                LocationId = model.LocationId,
                ScheduledArrival = model.ScheduledArrival,
                ScheduledDeparture = model.ScheduledDeparture,
                ActualArrival = model.ActualArrival ?? DateTime.MinValue,
                ActualDeparture = model.ActualDeparture ?? DateTime.MinValue,
                Remarks = model.Remarks ?? "",
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = Guid.Empty // Replace with actual user
            };

            _context.TripStops.Add(stop);
            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "stopChanged";
            return Content("<div class='success-message'>✅ Stop created successfully</div>");
        }

        // GET: Edit form
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var stop = await _context.TripStops
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);

            if (stop == null) return NotFound();

            var model = new TripStopViewModel
            {
                Id = stop.Id,
                TripId = stop.TripId,
                StopNumber = stop.StopNumber,
                LocationId = stop.LocationId,
                ScheduledArrival = stop.ScheduledArrival,
                ScheduledDeparture = stop.ScheduledDeparture,
                ActualArrival = stop.ActualArrival == DateTime.MinValue ? null : stop.ActualArrival,
                ActualDeparture = stop.ActualDeparture == DateTime.MinValue ? null : stop.ActualDeparture,
                Remarks = stop.Remarks
            };

            await LoadLocations();
            return PartialView("_TripStopForm", model);
        }

        // PUT: Edit
        [HttpPut]
        public async Task<IActionResult> Edit(Guid id, TripStopViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLocations();
                return PartialView("_TripStopForm", model);
            }

            var stop = await _context.TripStops.FindAsync(id);
            if (stop == null) return NotFound();

            stop.StopNumber = model.StopNumber;
            stop.LocationId = model.LocationId;
            stop.ScheduledArrival = model.ScheduledArrival;
            stop.ScheduledDeparture = model.ScheduledDeparture;
            stop.ActualArrival = model.ActualArrival ?? DateTime.MinValue;
            stop.ActualDeparture = model.ActualDeparture ?? DateTime.MinValue;
            stop.Remarks = model.Remarks ?? "";
            stop.UpdatedAt = DateTime.Now;
            stop.EditUserId = Guid.Empty; // Replace with actual user

            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "stopChanged";
            return Content("<div class='success-message'>✅ Stop updated successfully</div>");
        }

        // DELETE
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var stop = await _context.TripStops.FindAsync(id);
            if (stop == null) return NotFound();

            stop.IsDeleted = true;
            stop.UpdatedAt = DateTime.Now;
            stop.EditUserId = Guid.Empty; // Replace with actual user

            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "stopChanged";
            return Ok();
        }

        private async Task LoadLocations()
        {
            ViewBag.Locations = await _context.Locations
                .Where(l => l.IsActive && !l.IsDeleted)
                .OrderBy(l => l.LocationName)
                .ToListAsync();
        }
    }
}
