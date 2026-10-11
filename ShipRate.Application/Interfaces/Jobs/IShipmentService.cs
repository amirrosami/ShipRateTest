using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Jobs
{
    public interface IShipmentService
    {
        Task<ShipmentLegFormVm> BuildShipmentLegCreateVmAsync(Guid shipmentId);
        Task<ShipmentLegFormVm?> BuildShipmentLegEditVmAsync(Guid id);
        Task<(bool Success, string? Error, Guid Id)> CreateShipmentLegAsync(ShipmentLegFormVm vm, Guid currentUserId);
        Task<(bool Success, string? Error)> DeleteShipmentLegAsync(Guid id, Guid currentUserId);
        Task FillShipmentLegLookupsAsync(ShipmentLegFormVm vm);
        Task<ShipmentLegDeleteVm?> GetShipmentLegDeleteVmAsync(Guid id);
        Task<ShipmentLegDetailsVm?> GetShipmentLegDetailsAsync(Guid id);
        Task<PagedResultDto<ShipmentLegListItemVm>> GetShipmentLegListAsync(ShipmentLegFilterVm filter);
        Task<(bool Success, string? Error)> UpdateShipmentLegAsync(ShipmentLegFormVm vm, Guid currentUserId);
    }
}