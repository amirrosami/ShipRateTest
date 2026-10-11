using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface IDetentionService
    {
        Task<PagedResultDto<DetentionListItemVm>> GetPagedListAsync(DetentionSearchVm search);
        Task<List<DetentionItemListItemVm>> GetItemsAsync(Guid detentionId);
        Task<List<DetentionItemDetailListItemVm>> GetItemDetailsAsync(Guid detentionItemId);
        Task<List<SelectListItem>> GetDetentionStatusesAsync(CancellationToken cancellationToken = default);
        Task<DetentionCalculationResultVm> CreateAndCalculateAsync(CreateDetentionVm model, CancellationToken cancellationToken = default);
        Task<List<SelectListItem>> GetBLSelectListAsync(Guid? blId);
        Task<List<SelectListItem>> GetBookingSelectListAsync(Guid? bookingId);
        Task<DetentionStartBaseViewModel?> GetDetentionStartBaseAsync(Guid? blId, Guid? bookingId, int companyId);
        Task SaveDetentionStartBaseAsync(DetentionStartBaseViewModel model);
        Task<byte[]> GenerateDetentionReportPdfAsync(Guid detentionId, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateDetentionReportPdfFarsiAsync(Guid id, CancellationToken cancellationToken);
        Task<byte[]> GenerateDetentionStatementPdfAsync(Guid detentionId, CancellationToken cancellationToken = default);
        Task<bool> ValidateTariffRulesAsync(Guid blId, Guid tariffBookId);
        Task<List<string>> GetMissingContainerTypesAsync(Guid blId, Guid tariffBookId);
        Task<DetentionListItemVm> GetDetentionAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ValidateToDeleteAsync(Guid id);
        Task<bool> ValidateTariffRulesForBookingAsync(Guid bookingId, Guid tariffBookId);
        Task<List<string>> GetMissingContainerTypesForBookingAsync(Guid bookingId, Guid tariffBookId);
    }
}
