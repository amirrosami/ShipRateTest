using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class PortDistanceRepository : Repository<PortDistance>, IPortDistanceRepository
    {
        public PortDistanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PortDistance>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(m => m.FromPort)
                .Include(m => m.ToPort)
                .Include(m => m.TransportType)
                .ToListAsync();
        }
        public async Task<IQueryable<PortDistance>> GetAllAsQueryableAsync()
        {
            return _context.Set<PortDistance>()
                .Include(m => m.FromPort)
                .Include(m => m.ToPort)
                .Include(m => m.TransportType)
                .AsNoTracking();
        }
        public async Task<PortDistance?> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.FromPort)
                .Include(m => m.ToPort)
                .Include(m => m.TransportType)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

    }
}