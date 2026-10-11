using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class InspectionResult : SystemEntity
    {
        public string InspectionResultName { get; set; }
        public string InspectionResultLocalName { get; set; }
    }
}
