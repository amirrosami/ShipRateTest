using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteFTLCharge : BusinessEntity
    {
        public Guid ProvidedRateRouteFTLId { get; set; }
        public int ChargeTypeId { get; set; }
        public decimal Amount { get; set; }

        //Relations 
        public ProvidedRateRouteFTL? ProvidedRateRouteFTL { get; set; }
        public ChargeType? ChargeType { get; set; }
    }
}
