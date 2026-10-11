using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class QuotationStatus : SystemEntity
    {
        public string QuotationStatusName { get; set; }
        public string QuotationStatusLocalName { get; set; }
    }
}
