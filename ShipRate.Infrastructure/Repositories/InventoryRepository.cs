using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class InventoryRepository : Repository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Inventory>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(m => m.Company)
                .ThenInclude(x => x.Person)
                .ToListAsync();
        }

        public async Task<IQueryable<Inventory>> GetAllAsQueryableAsync()
        {
            return _context.Set<Inventory>()
                .Include(m => m.Company)
                .ThenInclude(x => x.Person)
                .AsNoTracking();
        }


        public async Task<Inventory?> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.Company)
                .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Inventory>> GetByCompanyIdAsync(int companyId)
        {
            return await _dbSet
                .Include(m => m.Company)
                .ThenInclude(x => x.Person)
                .Where(m => m.CompanyId == companyId)
                .ToListAsync();
        }
    }
}