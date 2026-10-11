using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface INOAService
    {
        Task<BLMatchResultDto> SearchBLAsync(string searchTerm, CancellationToken ct = default);
        Task<NOAFormViewModel> GetFormAsync(Guid id, CancellationToken ct = default);
        Task<NOAFormViewModel> GetNewFormForBLAsync(Guid blId, CancellationToken ct = default);
        Task<PagedResultDto<NOAListDto>> GetPageAsync(NOAFilterDto filter, CancellationToken ct = default);
        Task<SaveNOAResult> SaveAsync(NOAFormDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<List<NOAPriceDto>> CalculateAndGetPricesAsync(Guid noaId, CancellationToken ct = default);
        Task<List<NOAPriceDto>> GetPricesAsync(Guid noaId, CancellationToken ct = default);
        Task<ReferenceDataDto> GetReferenceDataAsync(Guid blId, CancellationToken ct = default);
        Task<NOASendResultDto> SendAsync(Guid noaId, int sendToolTypeId, string receiver, CancellationToken ct = default);
        Task<List<NOASendDto>> GetSendHistoryAsync(Guid noaId, CancellationToken ct = default);
        Task<NOAPrintViewModel?> GetPrintDataAsync(Guid noaId, CancellationToken ct);
        Task<PriceGridVM> SavePricesAsync(PriceGridVM model, CancellationToken cancellationToken);
        Task<PriceGridVM> GetPricesByNOAIdAsync(Guid nOAId, CancellationToken cancellationToken);
        Task<NOAFormDto> GetNOAHeaderAsync(Guid id, CancellationToken ct);
        Task<Guid> CreateOutFromInAsync(Guid inNoaId, CancellationToken ct);
        Task<List<NLookupDto>> GetChargeTypesAsync();
        Task<List<NLookupDto>> GetClearanceTypesAsync();
        Task AddPriceAsync(Guid noaId, NOAPriceDto dto, CancellationToken ct);
        Task DispatchNOANotificationAsync(NOASendRequestDto dto, string userId);
        Task<NOASendHubViewModel> GetSendHubViewModelAsync(Guid nOAId);
        Task<(byte[]? FileBytes, string? FileName)> GenerateNoaPdfBytesAsync(Guid id, CancellationToken ct);
    }
}
