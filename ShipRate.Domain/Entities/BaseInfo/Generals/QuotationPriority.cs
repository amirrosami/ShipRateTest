using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class QuotationPriority : SystemEntity
    {

        public string QuotationPriorityName { get; set; } = string.Empty;
    }
}