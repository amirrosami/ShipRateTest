using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Vehicles
{
    public class TripStatus : SystemEntity
    {
        public string TripStatusName { get; set; } = string.Empty;
        public string TripStatusLocalName { get; set; } = string.Empty;
    }
}
