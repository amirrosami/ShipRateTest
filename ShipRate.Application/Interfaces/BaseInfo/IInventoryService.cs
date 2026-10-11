using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IInventoryService
    {
        Task<PagedResultDto<InventoryDto>> GetPagedAsync(PagedRequestDto request);
        Task<InventoryDto?> GetByIdAsync(Guid id);

        Task<bool> SoftDeleteAsync(Guid id);

        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(InventoryCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(InventoryUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);
    }
}
