using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class InspectionMethod : SystemEntity
    {
        public string InspectionMethodName { get; set; }
        public string InspectionMethodLocalName { get; set; }
    }
}
