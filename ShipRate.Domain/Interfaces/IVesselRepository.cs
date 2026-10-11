using ShipRate.Domain.Entities.BaseInfo.Vehicles;

namespace ShipRate.Domain.Interfaces
{
    public interface IVesselRepository : IRepository<Vessel>
    {
        Task<IEnumerable<Vessel>> GetAllWithRelationsAsync();
        Task<Vessel?> GetByIdWithRelationsAsync(Guid id);
        Task<IQueryable<Vessel>> GetAllAsQueryableAsync();
    }
}
