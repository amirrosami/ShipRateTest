using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class ShippingLineRepository : Repository<Carrier>, IShippingLineRepository
    {
        public ShippingLineRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Carrier>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(x => x.Person)
                .Include(m => m.ShippingLineServices)
                .ThenInclude(x => x.Loops)
                .ThenInclude(x => x.LoopPorts)
                .Where(x => x.CarrierTypeId == 1)
                .ToListAsync();
        }
        public async Task<IQueryable<Carrier>> GetAllAsQueryableAsync()
        {
            return _context.Set<Carrier>()
                .Include(x => x.Person)
                .Include(m => m.ShippingLineServices)
                .ThenInclude(x => x.Loops)
                .ThenInclude(x => x.LoopPorts)
                .Where(x => x.CarrierTypeId == 1)
                .AsNoTracking();
        }
        public async Task<Carrier?> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Person)
                .Include(m => m.ShippingLineServices)
                .ThenInclude(x => x.Loops)
                .ThenInclude(x => x.LoopPorts)
                .Where(x => x.CarrierTypeId == 1)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
