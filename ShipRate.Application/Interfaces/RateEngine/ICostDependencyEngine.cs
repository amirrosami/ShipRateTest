using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Application.Interfaces.RateEngine
{
    public interface ICostDependencyEngine
    {
        Task<List<ChargeType>> ApplyDependenciesAsync(
                Guid shipmentId,
                List<ChargeType> baseCosts,
                CancellationToken cancellationToken);
    }
}
