using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteLTL : BusinessEntity
    {
        public Guid ProvidedRateRouteId { get; set; }
        public int ChargeTypeId { get; set; }
        public int MeasurementUnitId { get; set; }
        public decimal Amount { get; set; }
        //Relations 
        public ProvidedRateRoute? ProvidedRateRoute { get; set; }
        public ChargeType? ChargeType { get; set; }
        public MeasurementUnit? MeasurementUnit { get; set; }
    }
}
