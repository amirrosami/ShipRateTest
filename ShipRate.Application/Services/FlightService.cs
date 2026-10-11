using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public FlightService(ICurrentUserService currentUserService, ApplicationDbContext db)
        {
            _currentUserService = currentUserService;
            _db = db;
        }

        public async Task<IReadOnlyList<FlightDateLookupItemVm>> GetForFlightSelectAsync(Guid flightId, int take, CancellationToken cancellationToken)
        {
            return await _db.FlightDates
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.FlightId == flightId)
                .OrderBy(x => x.ETA).ThenBy(x => x.ActualArrival)
                .Take(take)
                .Select(x => new FlightDateLookupItemVm
                {
                    Id = x.Id,
                    ActualArrival = x.ActualArrival,
                    ETA = x.ETA,
                    ActualDeparture = x.ActualDeparture,
                    DelayHours = x.DelayHours,
                    ETD = x.ETD
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FlightLookupItemVm>> SearchForSelectAsync(string search, int take, CancellationToken cancellationToken)
        {
            search = search.Trim();
            return await _db.Flights
                .Include(x => x.AirLine)
                .Include(x => x.OriginAirPort)
                .Include(x => x.DestinationAirPort)
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            (x.FlightNumber.Contains(search)))
                .OrderBy(x => x.FlightNumber)
                .Take(take)
                .Select(x => new FlightLookupItemVm
                {
                    Id = x.Id,
                    AirlineId = x.AirLineId,
                    AirlineName = x.AirLine.Person.PersonName,
                    FlightNo = x.FlightNumber,
                    OriginAirportName = x.OriginAirPort != null ? x.OriginAirPort.LocationName : null,
                    DestinationAirportName = x.DestinationAirPort != null ? x.DestinationAirPort.LocationName : null,
                })
                .ToListAsync(cancellationToken);
        }
    }
}
