using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Interfaces
{
    public interface IPortDistanceRepository : IRepository<PortDistance>
    {
        Task<IEnumerable<PortDistance>> GetAllWithRelationsAsync();
        Task<PortDistance?> GetByIdWithRelationsAsync(Guid id);
        Task<IQueryable<PortDistance>> GetAllAsQueryableAsync();
    }
}
