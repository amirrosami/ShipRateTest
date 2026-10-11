using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Finance
{
    public class InvoiceType : SystemEntity
    {
        public string InvoiceTypeCode { get; set; }
        public string InvoiceTypeName { get; set; }
    }
}