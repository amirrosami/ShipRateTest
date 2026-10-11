using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface IFlightService
    {
        Task<IReadOnlyList<FlightDateLookupItemVm>> GetForFlightSelectAsync(Guid flightId, int take, CancellationToken cancellationToken);
        Task<IReadOnlyList<FlightLookupItemVm>> SearchForSelectAsync(string search, int take, CancellationToken cancellationToken);
    }
}
