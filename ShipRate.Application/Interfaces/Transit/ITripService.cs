using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces.Transit
{
    public interface ITripService
    {
        Task<IReadOnlyList<TripLookupItemVm>> SearchForSelectAsync(string search, int take, CancellationToken cancellationToken);
    }
}
