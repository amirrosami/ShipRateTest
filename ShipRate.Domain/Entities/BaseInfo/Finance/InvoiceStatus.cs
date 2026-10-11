using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class InvoiceStatus : SystemEntity
    {
        public int SortOrder { get; set; }
        public string InvoiceStatusName { get; set; } = string.Empty;
        public string InvoiceStatusLocalName { get; set; } = string.Empty;
    }
}