using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteAirTierCharge : BusinessEntity
    {
        public Guid ProvidedRateRouteAirTierId { get; set; }
        public int ChargeTypeId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPer { get; set; }
        public bool IsFixed { get; set; }
        //Relations 
        public ProvidedRateRouteAirTier? ProvidedRateRouteAirTier { get; set; }
        public ChargeType ChargeType { get; set; }
    }
}
