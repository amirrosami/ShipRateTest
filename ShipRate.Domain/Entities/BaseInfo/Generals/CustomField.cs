using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class CustomField : SystemEntity
    {
        public string CustomFieldName { get; set; }
        public string CustomFieldLocalName { get; set; }
        public string CustomFieldDataType { get; set; }
    }
}
