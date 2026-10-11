using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class MeasurementUnitRepository : Repository<MeasurementUnit>, IMeasurementUnitRepository
    {
        public MeasurementUnitRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MeasurementUnit>> GetAllWithCategoryAsync()
        {
            return await _dbSet
                .Include(m => m.MeasurementUnitCategory)
                .ToListAsync();
        }

        public async Task<IQueryable<MeasurementUnit>> GetAllAsQueryableAsync()
        {
            return _context.Set<MeasurementUnit>()
                .Include(m => m.MeasurementUnitCategory)
                .AsNoTracking();
        }


        public async Task<MeasurementUnit?> GetByIdWithCategoryAsync(int id)
        {
            return await _dbSet
                .Include(m => m.MeasurementUnitCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        //public async Task<MeasurementUnit?> GetByGuidWithCategoryAsync(Guid id)
        //{
        //    return await _dbSet
        //        .Include(m => m.MeasurementUnitCategory)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //}

        public async Task<IEnumerable<MeasurementUnit>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Include(m => m.MeasurementUnitCategory)
                .Where(m => m.MeasurementUnitCategoryId == categoryId)
                .ToListAsync();
        }
    }
}
