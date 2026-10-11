using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Vehicles
{
    public class TruckType : SystemEntity
    {
        public string TruckTypeName { get; set; } = string.Empty;
    }
}