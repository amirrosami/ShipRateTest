using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Jobs
{
    public interface IJobPartyService
    {
        Task<JobPartyFormVm> BuildCreateVmAsync(Guid jobId);
        Task<JobPartyFormVm?> BuildEditVmAsync(Guid id);
        Task<(bool Success, string? Error, Guid Id)> CreateAsync(JobPartyFormVm vm, Guid currentUserId);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id, Guid currentUserId);
        Task FillLookupsAsync(JobPartyFormVm vm);
        Task<JobPartyDeleteVm?> GetDeleteVmAsync(Guid id);
        Task<JobPartyDetailsVm?> GetDetailsAsync(Guid id);
        Task<PagedResultDto<JobPartyListItemVm>> GetListAsync(JobPartyFilterVm filter);
        Task<(bool Success, string? Error)> UpdateAsync(JobPartyFormVm vm, Guid currentUserId);
    }
}