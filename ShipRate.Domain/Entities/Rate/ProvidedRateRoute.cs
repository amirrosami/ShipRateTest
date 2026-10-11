using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Rate
{
    public class ProvidedRateRoute : BusinessEntity
    {
        public Guid ProvidedRateId { get; set; }
        public Guid RouteId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Remarks { get; set; }
        //Relations 
        public Route? Route { get; set; }
        public ProvidedRate ProvidedRate { get; set; }
        //public List<RatePeriod>? RatePeriods { get; set; }
    }
}
