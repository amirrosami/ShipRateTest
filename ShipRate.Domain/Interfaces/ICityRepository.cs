using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Interfaces
{
    public interface ICityRepository : IRepository<Location>
    {
        Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            int? countryId,
            string? sortColumn,
            string? sortDirection);

        Task<Location?> GetByIdWithCountryAsync(int id);
        Task<IEnumerable<Location>> GetByCountryIdAsync(int countryId);
    }
}
