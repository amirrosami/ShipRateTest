using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Locations;
using ShipRate.Application.Interfaces.Location;
using ShipRate.Domain.Enums;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Location
{
    /// <summary>
    /// پیاده‌سازی کامل ILocationService
    /// الگو: CityService + PermissionGroupService
    /// شامل: Manual Mapping + Validation + Error Handling
    /// </summary>
    public class LocationService : ILocationService
    {
        private const string LocationsCacheKey = "ShipRate_AllLocations_Lookup";

        private readonly ILocationRepository _locationRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IRepository<Domain.Entities.BaseInfo.Locations.Location> _provinceRepository;
        private readonly IRepository<Domain.Entities.BaseInfo.Locations.Location> _airportRepository;
        private readonly IRepository<Domain.Entities.BaseInfo.Locations.Location> _terminalRepository;
        private readonly IRepository<Domain.Entities.BaseInfo.Locations.Location> _depotRepository;
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;





        public LocationService(
            ILocationRepository locationRepository,
            ICountryRepository countryRepository,
            IRepository<Domain.Entities.BaseInfo.Locations.Location> provinceRepository,
            IRepository<Domain.Entities.BaseInfo.Locations.Location> airportRepository,
            IRepository<Domain.Entities.BaseInfo.Locations.Location> terminalRepository,
            IRepository<Domain.Entities.BaseInfo.Locations.Location> depotRepository,
            ApplicationDbContext db,
            IMemoryCache cache)
        {
            _locationRepository = locationRepository;
            _countryRepository = countryRepository;
            _provinceRepository = provinceRepository;
            _airportRepository = airportRepository;
            _terminalRepository = terminalRepository;
            _depotRepository = depotRepository;
            _db = db;
            _cache = cache;
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Paging & Search Implementation
        // ═══════════════════════════════════════════════════════════════════

        public async Task<PagedResultDto<LocationListItemDto>> GetPagedLocationsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            int? countryId = null,
            EnumLocationType? locationType = null,
            string? sortColumn = null,
            string? sortDirection = null)
        {
            var (items, totalCount) = await _locationRepository.GetPagedAsync(
                pageNumber, pageSize, searchTerm, countryId, locationType, sortColumn, sortDirection);

            var locationDtos = items.Select(loc => new LocationListItemDto
            {
                Id = loc.Id,
                LocationCode = loc.LocationCode,
                LocationName = loc.LocationName,
                LocationType = (EnumLocationType)loc.LocationTypeId,
                CountryId = loc.CountryId,
                CountryName = loc.Country?.CountryName ?? string.Empty,
                IsActive = loc.IsActive,
                CreatedDate = loc.CreatedAt
            });

            return new PagedResultDto<LocationListItemDto>
            {
                Items = locationDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Read Operations
        // ═══════════════════════════════════════════════════════════════════

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdWithCountryAsync(id);
            if (location == null) return null;

            return MapToDto(location);
        }

        public async Task<IEnumerable<LocationListItemDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return locations.Select(loc => new LocationListItemDto
            {
                Id = loc.Id,
                LocationCode = loc.LocationCode,
                LocationName = loc.LocationName,
                LocationType = (EnumLocationType)loc.LocationTypeId,
                CountryId = loc.CountryId,
                CountryName = loc.Country?.CountryName ?? string.Empty,
                IsActive = loc.IsActive,
                CreatedDate = loc.CreatedAt
            });
        }

        public async Task<IEnumerable<LocationListItemDto>> GetLocationsByCountryIdAsync(int countryId)
        {
            var locations = await _locationRepository.GetByCountryIdAsync(countryId);
            return locations.Select(loc => new LocationListItemDto
            {
                Id = loc.Id,
                LocationCode = loc.LocationCode,
                LocationName = loc.LocationName,
                LocationType = (EnumLocationType)loc.LocationTypeId,
                CountryId = loc.CountryId,
                CountryName = loc.Country?.CountryName ?? string.Empty,
                IsActive = loc.IsActive,
                CreatedDate = loc.CreatedAt
            });
        }

        public async Task<IEnumerable<LocationListItemDto>> GetLocationsByTypeAsync(EnumLocationType locationType)
        {
            var locations = await _locationRepository.GetByTypeAsync(locationType);
            return locations.Select(loc => new LocationListItemDto
            {
                Id = loc.Id,
                LocationCode = loc.LocationCode,
                LocationName = loc.LocationName,
                LocationType = (EnumLocationType)loc.LocationTypeId,
                CountryId = loc.CountryId,
                CountryName = loc.Country?.CountryName ?? string.Empty,
                IsActive = loc.IsActive,
                CreatedDate = loc.CreatedAt
            });
        }

        // ═══════════════════════════════════════════════════════════════════
        //  CRUD Operations
        // ═══════════════════════════════════════════════════════════════════

        public async Task<LocationDto> CreateLocationAsync(LocationDto locationDto)
        {
            // Validation: بررسی کشور معتبر باشد
            var country = await _countryRepository.GetByIdAsync(locationDto.CountryId);
            if (country == null)
                throw new InvalidOperationException("کشور انتخاب شده معتبر نیست.");

            // Validation: بررسی تکراری نبودن کد
            if (await _locationRepository.IsCodeExistsAsync(locationDto.LocationCode, locationDto.CountryId))
                throw new InvalidOperationException($"کد '{locationDto.LocationCode}' قبلاً در این کشور ثبت شده است.");

            //// Validation: بررسی IATA Code برای فرودگاه‌ها
            //if (locationDto.LocationType == EnumLocationType.AirPort
            //    && !string.IsNullOrWhiteSpace(locationDto.IataCode))
            //{
            //    if (await _locationRepository.IsIataCodeExistsAsync(locationDto.IataCode))
            //        throw new InvalidOperationException($"کد IATA '{locationDto.IataCode}' قبلاً استفاده شده است.");
            //}

            var location = new Domain.Entities.BaseInfo.Locations.Location
            {
                Id = Guid.CreateVersion7(),
                CountryId = locationDto.CountryId,
                LocationCode = locationDto.LocationCode,
                LocationName = locationDto.LocationName,
                LocationTypeId = (int)locationDto.LocationType,
                //IsActive = locationDto.IsActive,
                CreatedAt = DateTime.Now
            };

            var createdLocation = await _locationRepository.AddAsync(location);

            locationDto.Id = createdLocation.Id;
            locationDto.CreatedDate = createdLocation.CreatedAt;
            locationDto.CountryName = country.CountryName;

            InvalidateLocationCache();

            return locationDto;
        }

        public async Task<bool> UpdateLocationAsync(LocationDto locationDto)
        {
            var location = await _locationRepository.GetByIdAsync(locationDto.Id);
            if (location == null) return false;

            // Validation: بررسی کشور معتبر باشد
            var country = await _countryRepository.GetByIdAsync(locationDto.CountryId);
            if (country == null)
                throw new InvalidOperationException("کشور انتخاب شده معتبر نیست.");

            // Validation: بررسی تکراری نبودن کد (به جز خودش)
            if (await _locationRepository.IsCodeExistsAsync(
                locationDto.LocationCode,
                locationDto.CountryId,
                locationDto.Id))
            {
                throw new InvalidOperationException($"کد '{locationDto.LocationCode}' قبلاً در این کشور ثبت شده است.");
            }

            //// Validation: بررسی IATA Code (به جز خودش)
            //if (locationDto.LocationType == EnumLocationType.AirPort
            //    && !string.IsNullOrWhiteSpace(locationDto.IataCode))
            //{
            //    if (await _locationRepository.IsIataCodeExistsAsync(locationDto.IataCode, locationDto.Id))
            //        throw new InvalidOperationException($"کد IATA '{locationDto.IataCode}' قبلاً استفاده شده است.");
            //}

            location.CountryId = locationDto.CountryId;
            location.LocationCode = locationDto.LocationCode;
            location.LocationName = locationDto.LocationName;
            location.LocationTypeId = (int)locationDto.LocationType;
            location.IsActive = locationDto.IsActive;
            location.UpdatedAt = DateTime.Now;
            await _locationRepository.UpdateAsync(location);
            InvalidateLocationCache();
            return true;
        }

        public async Task<bool> DeleteLocationAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null) return false;

            // حذف منطقی (Soft Delete)
            location.IsActive = false;
            location.UpdatedAt = DateTime.Now;
            await _locationRepository.UpdateAsync(location);
            return true;
        }

        public async Task<bool> PermanentDeleteLocationAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null) return false;

            // حذف فیزیکی
            await _locationRepository.DeleteAsync(location);
            return true;
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Validation & Business Logic
        // ═══════════════════════════════════════════════════════════════════

        public async Task<bool> IsCodeUniqueAsync(string locationCode, int countryId, Guid? excludeId = null)
        {
            var exists = await _locationRepository.IsCodeExistsAsync(locationCode, countryId, excludeId);
            return !exists; // اگر وجود نداشته باشد یکتاست
        }

        //public async Task<bool> IsIataCodeUniqueAsync(string iataCode, Guid? excludeId = null)
        //{
        //    if (string.IsNullOrWhiteSpace(iataCode))
        //        return true; // کد خالی همیشه یکتاست

        //    var exists = await _locationRepository.IsIataCodeExistsAsync(iataCode, excludeId);
        //    return !exists;
        //}

        public async Task<bool> ToggleActiveStatusAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null) return false;

            location.IsActive = !location.IsActive;
            location.UpdatedAt = DateTime.Now;
            await _locationRepository.UpdateAsync(location);
            return true;
        }

        // ═══════════════════════════════════════════════════════════════════
        //  Private Helper: Entity → DTO Mapping
        // ═══════════════════════════════════════════════════════════════════

        private static LocationDto MapToDto(Domain.Entities.BaseInfo.Locations.Location location)
        {
            return new LocationDto
            {
                Id = location.Id,
                CountryId = location.CountryId,
                CountryName = location.Country?.CountryName,
                LocationCode = location.LocationCode,
                LocationName = location.LocationName,
                LocationType = (EnumLocationType)location.LocationTypeId,
                IsActive = location.IsActive,
                CreatedDate = location.CreatedAt,
                ModifiedDate = location.UpdatedAt
            };
        }


        public async Task<IEnumerable<NLookupDto>> SearchPortsAsync(string? term, int take = 20)
        {
            if (take <= 0) take = 20;
            if (take > 100) take = 100;

            var query = _db.Locations
                .AsNoTracking()
                .AsQueryable();

            // فیلتر کردن بر اساس عبارت جستجو شده
            if (!string.IsNullOrWhiteSpace(term))
            {
                var trimmedTerm = term.Trim();

                // جستجو همزمان روی PortCode (مانند CNSHA یا IRBND) و PortName
                query = query.Where(p =>
                    p.LocationName.Contains(trimmedTerm) ||
                    (p.LocationCode != null && p.LocationCode.Contains(trimmedTerm)));
            }

            return await query
                .OrderBy(p => p.LocationName)
                .Take(take)
                .Select(p => new NLookupDto
                {
                    Value = p.Id.ToString(), // تبدیل Id (چه Guid چه int) به string
                    Text = !string.IsNullOrEmpty(p.LocationCode)
                        ? $"{p.LocationCode} - {p.LocationName}"
                        : p.LocationName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<NLookupDto>> SearchPortsUsingCacheAsync(string term, int take, CancellationToken ct = default)
        {
            var allLocations = await GetAllLocationsCachedAsync(ct);

            if (string.IsNullOrWhiteSpace(term))
            {
                return allLocations.Take(take).ToList();
            }
            return allLocations
                .Where(l => l.Text.ToUpper().Contains(term.ToUpper()) || l.Text.ToLower().Contains(term.ToLower()))
                .OrderBy(l => l.Text)
                .Take(take)
                .ToList();
        }
        public void InvalidateLocationCache()
        {
            _cache.Remove(LocationsCacheKey);
        }
        private async Task<List<NLookupDto>> GetAllLocationsCachedAsync(CancellationToken ct)
        {
            // استفاده از GetOrCreateAsync
            return await _cache.GetOrCreateAsync(LocationsCacheKey, async entry =>
            {
                // تنظیم زمان انقضا (مثلاً 12 ساعت به صورت اسلایدینگ یا قطعی)
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
                entry.SlidingExpiration = TimeSpan.FromHours(4);
                entry.Priority = CacheItemPriority.High;

                // خواندن AsNoTracking و سبک از دیتابیس
                return await _db.Locations
                    .AsNoTracking()
                    .Select(l => new NLookupDto
                    {
                        Value = l.Id.ToString(),
                        Text = !string.IsNullOrEmpty(l.LocationCode)
                        ? $"{l.LocationCode} - {l.LocationName}"
                        : l.LocationName
                    })
                    .ToListAsync(ct);
            }) ?? new List<NLookupDto>();
        }



    }
}
