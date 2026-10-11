using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface ICountryService
    {
        Task<PagedResultDto<CountryDto>> GetPagedCountriesAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortColumn = null,
            string? sortDirection = null);

        Task<CountryDto?> GetCountryByIdAsync(int id);
        Task<CountryDto?> GetCountryWithCitiesAsync(int id);
        Task<IEnumerable<CountryDto>> GetAllCountriesAsync();
        Task<CountryDto> CreateCountryAsync(CountryDto countryDto);
        Task<bool> UpdateCountryAsync(CountryDto countryDto);
        Task<bool> DeleteCountryAsync(int id);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
    }
}
