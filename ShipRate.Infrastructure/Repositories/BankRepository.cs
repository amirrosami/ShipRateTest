using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class BankRepository : Repository<Bank>, IBankRepository
    {
        public BankRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bank>> GetAllWithCountryAsync()
        {
            return await _dbSet
                .Include(m => m.Country)
                .ToListAsync();
        }

        public async Task<IQueryable<Bank>> GetAllAsQueryableAsync()
        {
            return _context.Set<Bank>()
                .Include(m => m.Country)
                .AsNoTracking();
        }


        public async Task<Bank?> GetByIdWithCountryAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Bank>> GetByCountryIdAsync(int countryId)
        {
            return await _dbSet
                .Include(m => m.Country)
                .Where(m => m.CountryId == countryId)
                .ToListAsync();
        }
    }
}
