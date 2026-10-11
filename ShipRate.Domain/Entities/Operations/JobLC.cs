using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobLC : BusinessEntity
    {
        public Guid JobId { get; set; }
        public bool IsLCBased { get; set; }
        public string? LCNumber { get; set; }
        public string? LCIssuingBankName { get; set; }
        public DateTime? LCExpiryDate { get; set; }
        public DateTime? LCLatestShipDate { get; set; }//آخرین تاریخ مجاز حمل بر اساس ال سی

        //Relations
        public Job Job { get; set; }
    }
}
