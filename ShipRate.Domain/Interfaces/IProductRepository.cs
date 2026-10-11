using ShipRate.Domain.Entities.InventoryOperation;

namespace ShipRate.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product?> GetByIdWithCategoryAsync(Guid id);
        Task<IQueryable<Product>> GetAllAsQueryableAsync();
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    }
}
