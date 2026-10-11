using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;

namespace ShipRate.Application.Interfaces.Finance
{
    public interface IExchangeRateService
    {
        Task<PagedResultDto<ExchangeRateDto>> GetPagedAsync(PagedRequestDto request);
        Task<ExchangeRateDto?> GetByIdAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(ExchangeRateCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(ExchangeRateUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);
        Task<decimal> GetFormalPortsExchangeRateValueAsync(int fromCurrId, int toCurrId, DateTime date);
        Task<decimal> GetFreeMarketExchangeRateValueAsync(int fromCurrId, int toCurrId, DateTime date);
    }
}
