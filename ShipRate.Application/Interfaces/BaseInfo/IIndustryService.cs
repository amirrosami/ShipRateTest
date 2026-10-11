using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IIndustryService
    {
        Task<PagedResultDto<IndustryDto>> GetPagedAsync(PagedRequestDto request);
        Task<IndustryDto?> GetByIdAsync(Guid id);

        Task<bool> SoftDeleteAsync(Guid id);

        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(IndustryCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(IndustryUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);

    }
}
