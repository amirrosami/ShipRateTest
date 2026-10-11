using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRouteFTL : BusinessEntity
    {
        public Guid ProvidedRateRouteId { get; set; }
        public int TruckingWeightId { get; set; }
        public string Remarks { get; set; }
        //Relations 
        public ProvidedRateRoute? ProvidedRateRoute { get; set; }
    }
}
