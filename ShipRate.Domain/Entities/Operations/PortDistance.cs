using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
namespace ShipRate.Domain.Entities.Operations
{
    public class PortDistance : BusinessEntity
    {
        public Guid FromPortId { get; set; }
        public Guid ToPortId { get; set; }
        public int TransportTypeId { get; set; } // Sea, Road, Rail, Air
        public decimal? DistanceKM { get; set; }
        public decimal? DistanceNM { get; set; }   //Nautical Mile 
        public decimal? AvgTransitTimeDays { get; set; }
        public decimal? AvgTransitTimeHours { get; set; }
        public string Source { get; set; } = string.Empty;

        //Relations
        public Location FromPort { get; set; }
        public Location ToPort { get; set; }
        public TransportType TransportType { get; set; }
    }
}