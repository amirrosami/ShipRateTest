using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Vehicles
{
    public class VesselType : SystemEntity
    {
        public string VesselTypeName { get; set; } = string.Empty;
    }
}