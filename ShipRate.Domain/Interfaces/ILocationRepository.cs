using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Enums;

namespace ShipRate.Domain.Interfaces
{
    public interface ILocationRepository
    {
        // ═══════════════════════════════════════════════════════════════════
        //  Basic CRUD
        // ═══════════════════════════════════════════════════════════════════

        Task<Location?> GetByIdAsync(Guid id);
        Task<Location?> GetByIdWithCountryAsync(Guid id);
        Task<IEnumerable<Location>> GetAllAsync();
        Task<Location> AddAsync(Location location);
        Task UpdateAsync(Location location);
        Task DeleteAsync(Location location);

        // ═══════════════════════════════════════════════════════════════════
        //  Query Methods
        // ═══════════════════════════════════════════════════════════════════

        Task<IEnumerable<Location>> GetByCountryIdAsync(int countryId);
        Task<IEnumerable<Location>> GetByTypeAsync(EnumLocationType locationType);

        /// <summary>
        /// Paging با قابلیت فیلتر و Sort (برای Index Page)
        /// </summary>
        Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? countryId = null,
            EnumLocationType? locationType = null,
            string? sortColumn = null,
            string? sortDirection = null);

        // ═══════════════════════════════════════════════════════════════════
        //  Validation Helpers
        // ═══════════════════════════════════════════════════════════════════

        Task<bool> IsCodeExistsAsync(string locationCode, int countryId, Guid? excludeId = null);
    }
}
