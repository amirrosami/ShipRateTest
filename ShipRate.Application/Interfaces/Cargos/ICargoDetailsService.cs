using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Cargos
{
    public interface ICargoDetailsService
    {
        Task<CargoDetailFormVm> BuildCreateModelAsync(CargoDetailFilterVm filter);
        Task<CargoDetailDeleteVm?> BuildDeleteModelAsync(Guid id);
        Task<CargoDetailFormVm?> BuildEditModelAsync(Guid id);
        Task CreateAsync(CargoDetailFormVm model, Guid userId);
        Task<CargoDetailFilterVm?> DeleteAsync(Guid id, Guid userId);
        Task FillLookupsAsync(CargoDetailFormVm model);
        Task<CargoDetailDetailsVm?> GetDetailsAsync(Guid id);
        Task<List<CargoDetailListItemVm>> GetListAsync(CargoDetailFilterVm filter);
        Task<bool> PushDataFromJobToBLAsync(CargoDetailFilterVm vm);
        Task<bool> PushDataFromJobToBookingAsync(CargoDetailFilterVm vm);
        Task<bool> UpdateAsync(CargoDetailFormVm model, Guid userId);
    }
}