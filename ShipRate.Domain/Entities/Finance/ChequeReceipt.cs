using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class ChequeReceipt : BusinessEntity
    {

        public int CompanyId { get; set; }
        public Guid ChequeId { get; set; }
        public int ReceiptNo { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string Remarks { get; set; } = string.Empty;

        //Relations
        public Cheque? Cheque { get; set; }
        public Company Company { get; set; }
    }
}