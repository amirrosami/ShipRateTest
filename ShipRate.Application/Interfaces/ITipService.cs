using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface ITipService
    {
        Task<PagedResultDto<TipListDto>> GetPagedTipsAsync(TipFilterDto filter);

        Task<TipEditDto> GetTipForEditAsync(Guid id);
        Task<Guid> CreateTipAsync(TipEditDto dto, Guid currentUserId);
        Task UpdateTipAsync(TipEditDto dto, Guid currentUserId);
        Task DeleteTipAsync(Guid id, Guid currentUserId);

        Task<List<TipUsageListDto>> GetTipUsagesByTipAsync(Guid tipId);

        Task<TipUsageListDto> GetTipUsageByBlIdAsync(Guid blId);
        Task<List<TipUsageContainerListDto>> GetContainersByTipUsageAsync(Guid tipUsageId);

        Task<PagedResultDto<VoyagePortLookupDto>> LookupVoyagePortsAsync(VoyagePortLookupFilterDto filter);
        Task<VoyagePortLookupDto> GetVoyagePortByIdAsync(Guid voyagePortId);
        Task<int> GetNewSerialNoAsync(int CompanyId, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateTipLetterPdfAsync(TipUsageListDto dto);
    }
}
