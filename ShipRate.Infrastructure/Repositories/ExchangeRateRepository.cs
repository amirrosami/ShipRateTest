using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class ExchangeRateRepository : Repository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<ExchangeRate>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(m => m.FromCurrency)
                .Include(m => m.ToCurrency)
                .Include(m => m.ExchangeRateBase)
                .OrderByDescending(m => m.ExchangeRateDate)
                .ToListAsync();
        }

        public async Task<IQueryable<ExchangeRate>> GetAllAsQueryableAsync()
        {
            return _context.Set<ExchangeRate>()
                .Include(m => m.FromCurrency)
                .Include(m => m.ToCurrency)
                .Include(m => m.ExchangeRateBase)
                .OrderByDescending(m => m.ExchangeRateDate)
                .AsNoTracking();
        }
        public async Task<ExchangeRate?> GetByIdWithRelationsAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.FromCurrency)
                .Include(m => m.ToCurrency)
                .Include(m => m.ExchangeRateBase)
                .OrderByDescending(m => m.ExchangeRateDate)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
