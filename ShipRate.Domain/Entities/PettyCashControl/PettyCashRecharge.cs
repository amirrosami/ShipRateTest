using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.PettyCashControl
{
    public class PettyCashRecharge : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid PettyCashId { get; set; }
        public decimal RechargeAmount { get; set; }
        public DateTime? RechargeDate { get; set; }
        public string Description { get; set; } = null!;
        public Guid? ApprovedByUserId { get; set; }
        public Guid? RejectedByUserId { get; set; }
        public Guid? ChargedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int StatusId { get; set; }

        // Navigation Properties
        public Company Company { get; set; }
        public PettyCash PettyCash { get; set; } = null!;
        public ApplicationUser? ApprovedByUser { get; set; } = null!;
        public ApplicationUser? RejectedByUser { get; set; } = null!;
        public ApplicationUser? ChargedByUser { get; set; } = null!;
        public PettyCashRechargeStatus Status { get; set; }
    }
}
