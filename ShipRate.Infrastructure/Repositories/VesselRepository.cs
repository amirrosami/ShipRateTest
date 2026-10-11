using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class VesselRepository : Repository<Vessel>, IVesselRepository
    {
        public VesselRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Vessel>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(m => m.OwnType)
                .Include(m => m.FlagCountry)
                .Include(m => m.Owner)
                .Include(m => m.ShippingLine)
                .ThenInclude(m => m.Person)
                .Include(x => x.VesselType)
                .ToListAsync();
        }

        public async Task<IQueryable<Vessel>> GetAllAsQueryableAsync()
        {
            return _context.Set<Vessel>()
                .Include(m => m.OwnType)
                .Include(m => m.FlagCountry)
                .Include(m => m.Owner)
                .Include(m => m.ShippingLine)
                .ThenInclude(m => m.Person)
                .Include(x => x.VesselType)
                .AsNoTracking();
        }


        public async Task<Vessel?> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.OwnType)
                .Include(m => m.FlagCountry)
                .Include(m => m.Owner)
                .Include(m => m.ShippingLine)
                .ThenInclude(m => m.Person)
                .Include(x => x.VesselType)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
