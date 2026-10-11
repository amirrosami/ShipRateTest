using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteAirTier : BusinessEntity
    {
        public Guid ProvidedRateRouteAirId { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
        //Relations 
        public ProvidedRateRouteAir? ProvidedRateRouteAir { get; set; }
    }
}
