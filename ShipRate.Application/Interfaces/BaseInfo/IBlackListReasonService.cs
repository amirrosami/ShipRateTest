using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IBlackListReasonService
    {
        Task<PagedResultDto<BlackListReasonDto>> GetPagedAsync(PagedRequestDto request);
        Task<BlackListReasonDto?> GetByIdAsync(int id);
        Task<bool> SoftDeleteAsync(int id);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(BlackListReasonCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(BlackListReasonCreateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(int id);
    }
}
