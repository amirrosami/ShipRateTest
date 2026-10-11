using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class TransportType : SystemEntity
    {
        public string TransportTypeName { get; set; } = string.Empty;
    }
}
