using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface ITransportDocumentService
    {
        Task<BLManagePageVm> GetManagePageAsync(BLContextDto context);
        Task<List<BLListItemDto>> GetListAsync(BLContextDto context);
        Task<TransportDocumentUpsertVm> BuildCreateModelAsync(Guid jobId);
        Task<TransportDocumentUpsertVm> BuildEditModelAsync(Guid jobId, Guid id);

        Task<Guid> SaveAsync(Guid jobId, TransportDocumentUpsertVm model);

        Task DeleteAsync(Guid jobId, Guid id);
        Task IssueAsync(Guid jobId, Guid id);
        Task VoidAsync(Guid jobId, Guid id);
        Task<string?> GetPartyTextInBLAsync(Guid partyId);



        Task<AdvancedBLSplitVm?> GetAdvancedSplitWorkspaceAsync(Guid blId, Guid jobId);
        Task<ServiceResult<Guid>> SubmitAdvancedSplitAsync(SubmitAdvancedBLSplitRequestVm model);
        Task<List<BLLookupItemVm>> SearchBLsAsync(string? term);
        //Task<Guid?> GetJobIdAsync(Guid blId);


        Task<ContainerAllocationModalVm> GetContainerAllocationModalAsync(Guid blId, Guid roadTripId, CancellationToken cancellationToken = default);
        Task<ServiceResult<bool>> SaveContainerAllocationAsync(SaveContainerAllocationDto dto, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> CheckAndInsertMasterFileAsync(TransportDocumentUpsertVm model);
        Task<Guid?> GetPODTariffBookIdAsync(Guid blId);
        Task<Guid?> GetInvoiceClientIdAsync(Guid blId);
        Task<bool> BLHasCntAsync(Guid blId);
        Task<FinalVoyageVm> GetFinalVoyageForBLAsync(Guid blId);
        Task<Guid> GetFinalPortIdForBLAsync(Guid blId);
    }
}