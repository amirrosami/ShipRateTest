using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Vehicles
{
    public class GPSDeviceStatus : SystemEntity
    {
        public string GPSDeviceStatusName { get; set; } = string.Empty;
        public string GPSDeviceStatusLocalName { get; set; } = string.Empty;
    }
}
