using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<(IEnumerable<Country> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? sortColumn,
            string? sortDirection);

        Task<Country?> GetByIdWithCitiesAsync(int id);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
    }
}
