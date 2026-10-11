using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Enums;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Basic CRUD Implementation
        // ═══════════════════════════════════════════════════════════════════

        public async Task<Location?> GetByIdAsync(Guid id)
        {
            return await _context.Locations
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Location?> GetByIdWithCountryAsync(Guid id)
        {
            return await _context.Locations
                .Include(l => l.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations
                .Include(l => l.Country)
                .AsNoTracking()
                .OrderBy(l => l.LocationName)
                .ToListAsync();
        }

        public async Task<Location> AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Location location)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Query Methods Implementation
        // ═══════════════════════════════════════════════════════════════════

        public async Task<IEnumerable<Location>> GetByCountryIdAsync(int countryId)
        {
            return await _context.Locations
                .Include(l => l.Country)
                .Where(l => l.CountryId == countryId)
                .OrderBy(l => l.LocationName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetByTypeAsync(EnumLocationType locationType)
        {
            return await _context.Locations
                .Include(l => l.Country)
                .Where(l => l.LocationTypeId == (int)locationType)
                .OrderBy(l => l.LocationName)
                .AsNoTracking()
                .ToListAsync();
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Paging & Search Implementation (الگوی CityRepository)
        // ═══════════════════════════════════════════════════════════════════

        public async Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            int? countryId = null,
            EnumLocationType? locationType = null,
            string? sortColumn = null,
            string? sortDirection = null)
        {
            var query = _context.Locations
                .Include(l => l.Country)
                .AsNoTracking()
                .AsQueryable();

            // ─── فیلتر جستجو ───
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.LocationName.ToLower().Contains(term) ||
                    l.LocationCode.ToLower().Contains(term));
            }

            // ─── فیلتر کشور ───
            if (countryId.HasValue)
            {
                query = query.Where(l => l.CountryId == countryId.Value);
            }

            // ─── فیلتر نوع مکان ───
            if (locationType.HasValue)
            {
                query = query.Where(l => l.LocationTypeId == (int)locationType.Value);
            }

            // ─── شمارش کل ───
            var totalCount = await query.CountAsync();

            // ─── مرتب‌سازی ───
            query = ApplySorting(query, sortColumn, sortDirection);

            // ─── صفحه‌بندی ───
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Validation Helpers Implementation
        // ═══════════════════════════════════════════════════════════════════

        public async Task<bool> IsCodeExistsAsync(string locationCode, int countryId, Guid? excludeId = null)
        {
            var query = _context.Locations
                .Where(l => l.LocationCode == locationCode && l.CountryId == countryId);

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Private Helper: Sorting Logic
        // ═══════════════════════════════════════════════════════════════════

        private static IQueryable<Location> ApplySorting(
            IQueryable<Location> query,
            string? sortColumn,
            string? sortDirection)
        {
            var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return sortColumn?.ToLower() switch
            {
                "locationname" => isDescending
                    ? query.OrderByDescending(l => l.LocationName)
                    : query.OrderBy(l => l.LocationName),

                "locationcode" => isDescending
                    ? query.OrderByDescending(l => l.LocationCode)
                    : query.OrderBy(l => l.LocationCode),

                "locationtype" => isDescending
                    ? query.OrderByDescending(l => l.LocationType)
                    : query.OrderBy(l => l.LocationType),

                "countryname" => isDescending
                    ? query.OrderByDescending(l => l.Country!.CountryName)
                    : query.OrderBy(l => l.Country!.CountryName),

                "createddate" => isDescending
                    ? query.OrderByDescending(l => l.CreatedAt)
                    : query.OrderBy(l => l.CreatedAt),

                _ => query.OrderBy(l => l.LocationName) // Default Sort
            };
        }
    }
}
