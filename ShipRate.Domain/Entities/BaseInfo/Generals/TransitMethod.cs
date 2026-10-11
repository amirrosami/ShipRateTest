using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class TransitMethod : SystemEntity
    {
        public string TransitMethodName { get; set; } = string.Empty;
    }
}
