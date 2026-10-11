using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class CarrierType : SystemEntity
    {
        public string CarrierTypeName { get; set; } = string.Empty;
    }
}
