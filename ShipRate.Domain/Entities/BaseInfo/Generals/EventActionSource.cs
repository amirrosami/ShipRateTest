using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class EventActionSource : SystemEntity
    {
        public string EventActionSourceName { get; set; } = string.Empty;
    }
}
