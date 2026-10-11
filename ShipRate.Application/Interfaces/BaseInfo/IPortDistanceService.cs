using ShipRate.Application.DTOs;
using static ShipRate.Application.DTOs.PortDistanceDto;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IPortDistanceService
    {
        Task<PagedResultDto<PortDistanceDto>> GetPagedAsync(PagedRequestDto request);
        Task<PortDistanceDto?> GetByIdAsync(Guid id);

        Task<bool> SoftDeleteAsync(Guid id);

        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(PortDistanceCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(PortDistanceUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);
    }
}
