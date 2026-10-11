namespace ShipRate.Infrastructure.Repositories
{
    using global::ShipRate.Domain.Entities.BaseInfo.Locations;
    using global::ShipRate.Domain.Interfaces;
    using global::ShipRate.Infrastructure.Data;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    namespace ShipRate.Infrastructure.Repositories
    {
        public class CountryRepository : Repository<Country>, ICountryRepository
        {
            public CountryRepository(ApplicationDbContext context) : base(context)
            {
            }

            public Task<Country?> GetByIdWithCitiesAsync(int id)
            {
                throw new NotImplementedException();
            }

            public Task<(IEnumerable<Country> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, string? sortColumn, string? sortDirection)
            {
                throw new NotImplementedException();
            }

            public Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
            {
                throw new NotImplementedException();
            }

            //        public async Task<(IEnumerable<Country> Items, int TotalCount)> GetPagedAsync(
            //            int pageNumber,
            //            int pageSize,
            //            string? searchTerm,
            //            string? sortColumn,
            //            string? sortDirection)
            //        {
            //            var query = _dbSet.Include(c => c.Cities).AsQueryable();

            //            // Search
            //            if (!string.IsNullOrWhiteSpace(searchTerm))
            //            {
            //                searchTerm = searchTerm.Trim().ToLower();
            //                query = query.Where(c =>
            //                    c.CountryName.ToLower().Contains(searchTerm));
            //            }

            //            // Get total count
            //            var totalCount = await query.CountAsync();

            //            // Sort
            //            query = ApplySorting(query, sortColumn, sortDirection);

            //            // Pagination
            //            var items = await query
            //                .Skip((pageNumber - 1) * pageSize)
            //                .Take(pageSize)
            //                .ToListAsync();

            //            return (items, totalCount);
            //        }

            //        public async Task<Country?> GetByIdWithCitiesAsync(int id)
            //        {
            //            return await _dbSet
            //                .Include(c => c.Cities)
            //                .FirstOrDefaultAsync(c => c.Id == id);
            //        }

            //        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
            //        {
            //            var query = _dbSet.Where(c => c.CountryIsoCode2 == code);

            //            if (excludeId.HasValue)
            //            {
            //                query = query.Where(c => c.Id != excludeId.Value);
            //            }

            //            return !await query.AnyAsync();
            //        }

            //        private IQueryable<Country> ApplySorting(IQueryable<Country> query, string? sortColumn, string? sortDirection)
            //        {
            //            if (string.IsNullOrWhiteSpace(sortColumn))
            //            {
            //                return query.OrderBy(c => c.CountryName);
            //            }

            //            var isDescending = sortDirection?.ToLower() == "desc";

            //            query = sortColumn.ToLower() switch
            //            {
            //                "name" => isDescending ? query.OrderByDescending(c => c.CountryName) : query.OrderBy(c => c.CountryName),
            //                "code" => isDescending ? query.OrderByDescending(c => c.CountryIsoCode2) : query.OrderBy(c => c.CountryIsoCode2),
            //                _ => query.OrderBy(c => c.CountryName)
            //            };

            //            return query;
            //        }
            //    }
            //}
        }
    }
}