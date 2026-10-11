using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IBankService
    {
        Task<PagedResultDto<BankDto>> GetPagedAsync(PagedRequestDto request);
        Task<BankDto?> GetByIdAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(BankCreateEditDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(BankCreateEditDto dto);
    }
}
