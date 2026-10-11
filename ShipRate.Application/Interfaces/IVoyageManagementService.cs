using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface IVoyageManagementService
    {
        Task<VoyageManagementIndexViewModel> GetIndexModelAsync();
        Task<PagedResultDto<VoyageMonitoringRowDto>> SearchAsync(VoyageSearchRequestDto request);
        Task<VoyageMonitoringDetailsDto?> GetTimelineAsync(Guid voyagePortId);
        Task ExecuteActionAsync(VoyageActionRequestDto request);
        Task<VoyageMonitoringRowDto?> GetMonitoringRowAsync(Guid voyagePortId);
        Task<IReadOnlyList<VoyageLookupItemVm>> SearchForSelectAsync(string search, int take, CancellationToken cancellationToken);
        Task<IEnumerable<NLookupDto>> SearchPortsAsync(string term, int take);
    }
}
