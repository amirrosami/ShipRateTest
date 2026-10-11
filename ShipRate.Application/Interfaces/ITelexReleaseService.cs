using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface ITelexReleaseService
    {
        Task<TelexReleaseIndexViewModel> GetIndexAsync(TelexReleaseSearchViewModel search);
        Task<PagedResultDto<TelexReleaseListItemViewModel>> SearchAsync(TelexReleaseSearchViewModel search);

        Task<TelexReleaseCreateEditViewModel> GetCreateModelAsync(Guid? blId);
        Task<TelexReleaseCreateEditViewModel?> GetEditModelAsync(Guid id);
        Task<bool> CreateAsync(TelexReleaseCreateEditViewModel model, Guid userId);
        Task<bool> UpdateAsync(TelexReleaseCreateEditViewModel model, Guid userId);

        Task<TelexReleaseDetailsViewModel?> GetDetailsAsync(Guid id);

        Task<TelexReleaseCreateEditViewModel?> GetDeleteModelAsync(Guid id);
        Task<bool> DeleteAsync(Guid id, Guid userId);

        Task<BLLookupViewModel> SearchBLAsync(string? searchTerm);
        Task<List<SelectListItem>> GetStatusSelectListAsync();
        Task<List<SelectListItem>> GetLineForwarderTypeSelectListAsync();
        Task<TelexReleaseCreateEditViewModel?> GetApproveModelAsync(Guid id);
        Task<bool> ApproveAsync(Guid id, Guid userId);
        Task<bool> HasBLConfirmedTelexAsync(Guid blId);
    }
}
