namespace ShipRate.Application.Services
{
    using global::ShipRate.Application.DTOs;
    using global::ShipRate.Application.Interfaces;
    using global::ShipRate.Domain.Interfaces;

    namespace ShipRate.Application.Services
    {
        public class CityService : ICityService
        {
            private readonly ICityRepository _cityRepository;

            public CityService(ICityRepository cityRepository)
            {
                _cityRepository = cityRepository;
            }

            public async Task<PagedResultDto<CityDto>> GetPagedCitiesAsync(
                int pageNumber = 1,
                int pageSize = 10,
                string? searchTerm = null,
                int? countryId = null,
                string? sortColumn = null,
                string? sortDirection = null)
            {
                var (items, totalCount) = await _cityRepository.GetPagedAsync(
                    pageNumber, pageSize, searchTerm, countryId, sortColumn, sortDirection);

                var cityDtos = items.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.LocationName,
                    //CountryId = c.CountryId,
                    //CountryName = c.Country?.Name,
                    //Population = c.Population,
                    //Area = c.Area,
                    //IsCapital = c.IsCapital,
                    //IsActive = c.IsActive,
                    //CreatedDate = c.CreatedDate,
                    //ModifiedDate = c.ModifiedDate
                });

                return new PagedResultDto<CityDto>
                {
                    Items = cityDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection
                };
            }

            public async Task<CityDto?> GetCityByIdAsync(int id)
            {
                var city = await _cityRepository.GetByIdWithCountryAsync(id);
                if (city == null) return null;

                return new CityDto
                {
                    Id = city.Id,
                    Name = city.LocationName,
                    CountryId = city.CountryId,
                    CountryName = city.Country?.CountryName,
                };
            }

            public async Task<IEnumerable<CityDto>> GetAllCitiesAsync()
            {
                var cities = await _cityRepository.GetAllAsync();
                return cities.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.LocationName,
                    CountryId = c.CountryId,
                    CountryName = c.Country?.CountryName,
                });
            }

            public async Task<IEnumerable<CityDto>> GetCitiesByCountryIdAsync(int countryId)
            {
                var cities = await _cityRepository.GetByCountryIdAsync(countryId);
                return cities.Select(c => new CityDto
                {
                    Id = c.Id,
                    Name = c.LocationName,
                    CountryId = c.CountryId,
                    CountryName = c.Country?.CountryName,
                    //Population = c.Population,
                    //Area = c.Area,
                    //IsCapital = c.IsCapital,
                    //IsActive = c.IsActive,
                    //CreatedDate = c.CreatedDate,
                    //ModifiedDate = c.ModifiedDate
                });
            }

            public async Task<CityDto> CreateCityAsync(CityDto cityDto)
            {
                var city = new Domain.Entities.BaseInfo.Locations.Location
                {
                    //Name = cityDto.Name,
                    CountryId = cityDto.CountryId,
                };

                var createdCity = await _cityRepository.AddAsync(city);

                cityDto.Id = createdCity.Id;
                return cityDto;
            }

            public async Task<bool> UpdateCityAsync(CityDto cityDto)
            {
                var city = await _cityRepository.GetByGuidAsync(cityDto.Id);
                if (city == null) return false;

                //city.Name = cityDto.Name;
                city.CountryId = cityDto.CountryId;
                //city.Population = cityDto.Population;
                //city.Area = cityDto.Area;
                //city.IsCapital = cityDto.IsCapital;
                //city.IsActive = cityDto.IsActive;
                //city.ModifiedDate = DateTime.Now;

                await _cityRepository.UpdateAsync(city);
                return true;
            }

            public async Task<bool> DeleteCityAsync(int id)
            {
                var city = await _cityRepository.GetByIdAsync(id);
                if (city == null) return false;

                await _cityRepository.DeleteAsync(city);
                return true;
            }
        }
    }
}
