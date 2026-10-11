using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class TransitType : SystemEntity
    {
        public string TransitTypeName { get; set; } = string.Empty;
    }
}
