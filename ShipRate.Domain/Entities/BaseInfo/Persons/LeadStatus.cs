using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class LeadStatus : SystemEntity
    {
        public string LeadStatusName { get; set; }
        public string LeadStatusLocalName { get; set; }
    }
}
