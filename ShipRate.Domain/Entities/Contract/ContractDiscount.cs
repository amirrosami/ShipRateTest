//این کلاس کار دارد 
using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractDiscount : BusinessEntity
    {
        public Guid ContractRateId { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime RequsetDate { get; set; }
        public Guid RequestedById { get; set; }
        public string RequestRemarks { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? RejectDate { get; set; }
        public Guid ConfirmRejectedById { get; set; }
        public string ConfirmRejectRemarks { get; set; }
        public int StatusId { get; set; }

        //Relations
        public ContractRate ContractRate { get; set; }
        public ApplicationUser? RequestedBy { get; set; }
        public ApplicationUser? ConfirmRejectedBy { get; set; }

    }
}


/*status پر شود
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */