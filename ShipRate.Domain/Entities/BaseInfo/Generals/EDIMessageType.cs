using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class EDIMessageType : SystemEntity
    {
        public string EDIMessageTypeCode { get; set; }
        public string EDIMessageTypeName { get; set; }
        public string EDIMessageTypeStandard { get; set; }
    }
}
