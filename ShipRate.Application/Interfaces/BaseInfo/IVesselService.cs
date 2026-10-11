using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IVesselService
    {
        Task<PagedResultDto<VesselDto>> GetPagedAsync(PagedRequestDto request);
        Task<VesselDto?> GetByIdAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(VesselCreateEditDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(VesselCreateEditDto dto);
    }
}
