using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Transit
{
    public interface ITIRCarnetService
    {
        Task<TIRCarnetIndexVm> GetIndexVmAsync(TIRCarnetSearchVm search);
        Task<PagedResultDto<TIRCarnetListItemVm>> SearchAsync(TIRCarnetSearchVm search);

        Task<TIRCarnetCreateEditVm> GetCreateModelAsync();
        Task<TIRCarnetCreateEditVm?> GetEditModelAsync(Guid id);
        Task<TIRCarnetDeleteVm?> GetDeleteModelAsync(Guid id);
        Task<Guid> CreateAsync(TIRCarnetCreateEditVm model);
        Task<bool> UpdateAsync(TIRCarnetCreateEditVm model);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> ChangeStatusAsync(Guid id, int statusId);

        Task<AssignmentsPanelVm?> GetAssignmentsPanelAsync(Guid tirCarnetId);
        Task<TIRAssignmentCreateEditVm?> GetAssignmentCreateModelAsync(Guid tirCarnetId);
        Task<TIRAssignmentCreateEditVm?> GetAssignmentEditModelAsync(Guid id);
        Task<TIRAssignmentDeleteVm?> GetAssignmentDeleteModelAsync(Guid id);
        Task<Guid> CreateAssignmentAsync(TIRAssignmentCreateEditVm model);
        Task<bool> UpdateAssignmentAsync(TIRAssignmentCreateEditVm model);
        Task<bool> DeleteAssignmentAsync(Guid id);

        Task<UsagesPanelVm?> GetUsagesPanelAsync(Guid tirAssignmentId);
        Task<TIRUsageCreateEditVm?> GetUsageCreateModelAsync(Guid tirAssignmentId);
        Task<TIRUsageCreateEditVm?> GetUsageEditModelAsync(Guid id);
        Task<TIRUsageDeleteVm?> GetUsageDeleteModelAsync(Guid id);
        Task<Guid> CreateUsageAsync(TIRUsageCreateEditVm model);
        Task<bool> UpdateUsageAsync(TIRUsageCreateEditVm model);
        Task<bool> DeleteUsageAsync(Guid id);
    }
}
