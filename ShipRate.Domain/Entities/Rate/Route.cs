using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Rate
{
    public class Route : BusinessEntity
    {
        public string RouteName { get; set; } = string.Empty;
        public Guid OriginId { get; set; }
        public Guid DestinationId { get; set; }
        public int RouteModeId { get; set; }
        public decimal? DistanceMiles { get; set; }
        public decimal? DistanceKms { get; set; }
        public decimal? TransitTime { get; set; }
        //Relations
        public Location? Origin { get; set; }
        public Location? Destination { get; set; }
        //public List<ProvidedRateRoute>? RateRoutes { get; set; }
        public RouteMode? RouteMode { get; set; }

    }
}
