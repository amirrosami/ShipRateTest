using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class InspectionType : SystemEntity
    {
        public string InspectionTypeName { get; set; }
        public string InspectionTypeLocalName { get; set; }
    }
}
