using ShipRate.Domain.Entities.InventoryOperation;

namespace ShipRate.Domain.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<IEnumerable<Inventory>> GetAllWithRelationsAsync();
        Task<Inventory?> GetByIdWithRelationsAsync(Guid id);
        Task<IQueryable<Inventory>> GetAllAsQueryableAsync();
        Task<IEnumerable<Inventory>> GetByCompanyIdAsync(int companyId);
    }
}
