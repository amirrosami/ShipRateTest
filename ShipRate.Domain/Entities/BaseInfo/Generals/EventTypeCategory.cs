using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class EventTypeCategory : SystemEntity
    {
        public string EventTypeCategoryName { get; set; } = string.Empty;
        public string EventTypeCategoryLocalName { get; set; } = string.Empty;
        public bool? SystemDefinded { get; set; }
    }
}
