using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Cargos
{
    public interface IPackagesService
    {
        Task<RateRequestPackageFormVm> BuildCreateVmAsync(RateRequestPackageFilterVm filter);
        Task<RateRequestPackageFormVm?> BuildEditVmAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(RateRequestPackageFormVm model, Guid userId);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id, Guid userId);
        Task FillLookupsAsync(RateRequestPackageFormVm vm);
        Task<RateRequestPackageDeleteVm?> GetDeleteVmAsync(Guid id);
        Task<RateRequestPackageDetailsVm?> GetDetailsAsync(Guid id);
        Task<List<RateRequestPackageListItemVm>> GetListAsync(RateRequestPackageFilterVm filter);
        Task<(bool Success, string? Error)> UpdateAsync(RateRequestPackageFormVm model, Guid userId);
    }
}