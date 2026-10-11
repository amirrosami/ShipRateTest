using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class ActivityType : SystemEntity
    {
        public string ActivityTypeName { get; set; } = string.Empty;
        public string ActivityTypeLocalName { get; set; } = string.Empty;
    }
}
