using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteContainerCharge : BusinessEntity
    {
        public Guid ProvidedRateRouteContainerId { get; set; }
        public int ChargeTypeId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        //Relations
        public ProvidedRateRoute ProvidedRateRouteContainer { get; set; }
        public ChargeType? ChargeType { get; set; }
        public Currency Currency { get; set; }
    }
}