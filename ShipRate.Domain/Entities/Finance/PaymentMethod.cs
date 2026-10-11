using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Finance
{
    public class PaymentMethod : SystemEntity
    {
        public string PaymentMethodName { get; set; } = string.Empty;
        public string PaymentMethodLocalName { get; set; } = string.Empty;
        public string? PaymentMethodRemarks { get; set; } = string.Empty;
    }
}
