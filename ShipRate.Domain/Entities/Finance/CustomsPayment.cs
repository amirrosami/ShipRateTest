using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Customs;
using ShipRate.Domain.Entities.PettyCashControl;

namespace ShipRate.Domain.Entities.Finance
{
    public class CustomsPayment : BusinessEntity
    {
        public Guid SettlementId { get; set; }
        public Guid CustomsClearanceId { get; set; }
        public Guid? PaymentDocumentId { get; set; }
        public Guid? PettyCashId { get; set; }
        public string PaymentType { get; set; }  //DUTY, VAT, COMMERCIAL_BENEFIT, PENALTY, WAREHOUSE_RENT, INSPECTION_FEE, OTHER
        public decimal Amount { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Settlement Settlement { get; set; }
        public CustomsClearance CustomsClearance { get; set; }
        public PaymentDocument PaymentDocument { get; set; }
        public PettyCash PettyCash { get; set; }

    }
}
