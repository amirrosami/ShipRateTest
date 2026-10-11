using ShipRate.Domain.Entities.Base;
namespace ShipRate.Domain.Entities.Finance
{
    public class PaymentDocumentExtraInfo : BusinessEntity
    {
        public Guid PaymentDocumentId { get; set; }
        public string ExtraInfoKey { get; set; }
        public string ExtraInfoValue { get; set; }

        //Relations
        public PaymentDocument? PaymentDocument { get; set; }
    }
}
