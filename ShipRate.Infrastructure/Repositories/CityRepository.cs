using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{

    public class CityRepository : Repository<Domain.Entities.BaseInfo.Locations.Location>, ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Domain.Entities.BaseInfo.Locations.Location> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            int? countryId,
            string? sortColumn,
            string? sortDirection)
        {
            var query = _dbSet.Include(c => c.Country).AsQueryable();

            // Filter by country
            if (countryId.HasValue && countryId.Value > 0)
            {
                query = query.Where(c => c.CountryId == countryId.Value);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.LocationName.ToLower().Contains(searchTerm) ||
                    c.Country.CountryName.ToLower().Contains(searchTerm));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Sort
            query = ApplySorting(query, sortColumn, sortDirection);

            // Pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Domain.Entities.BaseInfo.Locations.Location?> GetByIdWithCountryAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Country)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Domain.Entities.BaseInfo.Locations.Location>> GetByCountryIdAsync(int countryId)
        {
            return await _dbSet
                .Include(c => c.Country)
                .Where(c => c.CountryId == countryId)
                .OrderBy(c => c.LocationName)
                .ToListAsync();
        }

        private IQueryable<Domain.Entities.BaseInfo.Locations.Location> ApplySorting(IQueryable<Domain.Entities.BaseInfo.Locations.Location> query, string? sortColumn, string? sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
            {
                return query.OrderBy(c => c.LocationName);
            }

            var isDescending = sortDirection?.ToLower() == "desc";

            query = sortColumn.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(c => c.LocationName) : query.OrderBy(c => c.LocationName),
                "country" => isDescending ? query.OrderByDescending(c => c.Country.CountryName) : query.OrderBy(c => c.Country.CountryName),
                _ => query.OrderBy(c => c.LocationName)
            };

            return query;
        }

        public Task<Domain.Entities.BaseInfo.Locations.Location?> GetByIdWithCountryAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
