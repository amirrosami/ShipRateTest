using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<PagedResultDto<CountryDto>> GetPagedCountriesAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortColumn = null,
            string? sortDirection = null)
        {
            var (items, totalCount) = await _countryRepository.GetPagedAsync(
                pageNumber, pageSize, searchTerm, sortColumn, sortDirection);

            var countryDtos = items.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.CountryName,
                //CitiesCount = c.Cities?.Count ?? 0
            });

            return new PagedResultDto<CountryDto>
            {
                Items = countryDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
        }

        public async Task<CountryDto?> GetCountryByIdAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.CountryName,
                //Code = country.Code,
                //Capital = country.Capital,
                //Population = country.Population,
                //Area = country.Area,
                //IsActive = country.IsActive,
                //CreatedDate = country.CreatedDate,
                //ModifiedDate = country.ModifiedDate,
                //CitiesCount = country.Cities?.Count ?? 0
            };
        }

        public async Task<CountryDto?> GetCountryWithCitiesAsync(int id)
        {
            var country = await _countryRepository.GetByIdWithCitiesAsync(id);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.CountryName,
                //Code = country.Code,
                //Capital = country.Capital,
                //Population = country.Population,
                //Area = country.Area,
                //IsActive = country.IsActive,
                //CreatedDate = country.CreatedDate,
                //ModifiedDate = country.ModifiedDate,
                //CitiesCount = country.Cities?.Count ?? 0
            };
        }

        public async Task<IEnumerable<CountryDto>> GetAllCountriesAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            return countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.CountryName,
                //Code = c.Code,
                //Capital = c.Capital,
                //Population = c.Population,
                //Area = c.Area,
                //IsActive = c.IsActive,
                //CreatedDate = c.CreatedDate,
                //ModifiedDate = c.ModifiedDate,
                //CitiesCount = c.Cities?.Count ?? 0
            });
        }

        public async Task<CountryDto> CreateCountryAsync(CountryDto countryDto)
        {
            var country = new Country
            {
                CountryName = countryDto.Name,
                //Code = countryDto.Code,
                //Capital = countryDto.Capital,
                //Population = countryDto.Population,
                //Area = countryDto.Area,
                //IsActive = countryDto.IsActive,
            };

            var createdCountry = await _countryRepository.AddAsync(country);

            countryDto.Id = createdCountry.Id;
            return countryDto;
        }

        public async Task<bool> UpdateCountryAsync(CountryDto countryDto)
        {
            var country = await _countryRepository.GetByIdAsync(countryDto.Id);
            if (country == null) return false;

            country.CountryName = countryDto.Name;

            await _countryRepository.UpdateAsync(country);
            return true;
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null) return false;

            await _countryRepository.DeleteAsync(country);
            return true;
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            return await _countryRepository.IsCodeUniqueAsync(code, excludeId);
        }
    }
}
