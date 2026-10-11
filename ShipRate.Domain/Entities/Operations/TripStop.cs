using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Operations
{
    public class TripStop : BusinessEntity
    {
        public Guid TripId { get; set; }
        public int StopNumber { get; set; }
        public Guid LocationId { get; set; }
        public DateTime? ScheduledArrival { get; set; }
        public DateTime? ScheduledDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        //Relations
        public Trip Trip { get; set; }
        public Location Location { get; set; }
    }
}