using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class PlanStatus : SystemEntity
    {
        public string StatusName { get; set; }
        public string StatusLocalName { get; set; }
    }
}
