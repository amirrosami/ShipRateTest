using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Finance
{
    public class FinanceDirection : SystemEntity
    {
        public string FinanceDirectionName { get; set; } = string.Empty;
        public string FinanceDirectionLocalName { get; set; } = string.Empty;
        public string ChargeContextName { get; set; } = string.Empty;
        public string InvoiceContextName { get; set; } = string.Empty;
        public string PaymentDocumentContextName { get; set; } = string.Empty;
    }
}
