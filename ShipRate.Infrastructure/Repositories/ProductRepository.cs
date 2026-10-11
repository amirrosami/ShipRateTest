using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _dbSet
                .Include(m => m.ProductCategory)
                .ToListAsync();
        }

        public async Task<IQueryable<Product>> GetAllAsQueryableAsync()
        {
            return _context.Set<Product>()
                .Include(m => m.ProductCategory)
                .AsNoTracking();
        }


        public async Task<Product?> GetByIdWithCategoryAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.ProductCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(m => m.ProductCategory)
                .Where(m => m.ProductCategoryId == categoryId)
                .ToListAsync();
        }
    }
}
