using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IMeasurementUnitService
    {
        Task<IEnumerable<MeasurementUnitCategory>> GetCategoriesAsync();
        Task<PagedResultDto<MeasurementUnitDTO>> GetPagedAsync(PagedRequestDto request);
        Task<PagedResultDto<MeasurementUnitDTO>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir);
        Task<MeasurementUnitDTO?> GetByIdAsync(int id);

        Task<bool> SoftDeleteAsync(int id);
        //Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        //Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);

        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(MeasurementUnitCreateDTO dto);
        Task<(bool Success, string? Error)> UpdateAsync(MeasurementUnitCreateDTO dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(int id);


    }
}
