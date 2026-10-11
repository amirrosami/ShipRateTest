using ShipRate.Domain.Entities.Lines;

namespace ShipRate.Domain.Interfaces
{
    public interface IShippingLineRepository : IRepository<Carrier>
    {
        Task<IEnumerable<Carrier>> GetAllWithRelationsAsync();
        Task<Carrier?> GetByIdWithRelationsAsync(Guid id);
        Task<IQueryable<Carrier>> GetAllAsQueryableAsync();
    }
}
