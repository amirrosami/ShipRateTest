using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface ICityService
    {
        Task<PagedResultDto<CityDto>> GetPagedCitiesAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            int? countryId = null,
            string? sortColumn = null,
            string? sortDirection = null);

        Task<CityDto?> GetCityByIdAsync(int id);
        Task<IEnumerable<CityDto>> GetAllCitiesAsync();
        Task<IEnumerable<CityDto>> GetCitiesByCountryIdAsync(int countryId);
        Task<CityDto> CreateCityAsync(CityDto cityDto);
        Task<bool> UpdateCityAsync(CityDto cityDto);
        Task<bool> DeleteCityAsync(int id);
    }
}
