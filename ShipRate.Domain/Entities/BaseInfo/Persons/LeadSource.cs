using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class LeadSource : SystemEntity
    {
        public string LeadSourceName { get; set; } = string.Empty;
    }
}
