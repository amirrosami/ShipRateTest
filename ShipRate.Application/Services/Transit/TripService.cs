using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Transit;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Transit
{
    public class TripService : ITripService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public TripService(ICurrentUserService currentUserService, ApplicationDbContext db)
        {
            _currentUserService = currentUserService;
            _db = db;
        }
        public async Task<IReadOnlyList<TripLookupItemVm>> SearchForSelectAsync(string search, int take, CancellationToken cancellationToken)
        {
            search = search.Trim();

            return await _db.Trips
                .Include(x => x.Truck)
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            (x.Truck.TrailerPlateNumber.Contains(search) ||
                             x.TripNumber.Contains(search)))
                .OrderBy(x => x.TripNumber)
                .Take(take)
                .Select(x => new TripLookupItemVm
                {
                    Id = x.Id,
                    TruckNo = x.Truck.TruckPlateNumber,
                    TrailerNo = x.Truck.TrailerPlateNumber
                })
                .ToListAsync(cancellationToken);
        }
    }
}
