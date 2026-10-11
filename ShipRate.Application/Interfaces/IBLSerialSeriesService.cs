using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IBLSerialSeriesService
    {
        Task<BLSerialSeriesIndexViewModel> GetIndexViewModelAsync();
        Task<List<BLSerialSeriesListItemDto>> SearchSeriesAsync(BLSerialSeriesSearchDto input);
        Task<BLSerialSeriesEditDto> GetSeriesForEditAsync(Guid? id);
        Task<Guid> CreateOrUpdateSeriesAsync(BLSerialSeriesEditDto dto, Guid currentUserId);
        Task DeleteSeriesAsync(Guid id, Guid currentUserId);

        Task<List<BLSerialBatchAssignListItemDto>> GetBatchAssignsAsync(Guid seriesId);
        Task<BLSerialBatchAssignEditDto> GetBatchAssignForEditAsync(Guid? id, Guid seriesId);
        Task<Guid> CreateOrUpdateBatchAssignAsync(BLSerialBatchAssignEditDto dto, Guid currentUserId);
        Task DeleteBatchAssignAsync(Guid id, Guid currentUserId);

        Task<List<BLSerialListItemDto>> GetSerialsAsync(Guid seriesId);
    }
}
