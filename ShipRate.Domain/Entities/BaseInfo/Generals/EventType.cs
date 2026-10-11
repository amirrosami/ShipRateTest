using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class EventType : SystemEntity
    {
        public int? ObjectTypeId { get; set; }
        public int? CategoryId { get; set; }
        public string EventTypeCode { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string EventTypeRemarks { get; set; } = string.Empty;
        public bool SystemDefined { get; set; } = false;
        public bool IsSystemEvent { get; set; }


        //Relations
        public SystemObjectType? ObjectType { get; set; }
        public EventTypeCategory Category { get; set; }
        //public List<VoyagePortEvent>? VoyagePortEvent { get; set; }
    }
}
