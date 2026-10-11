using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.PettyCashControl
{
    public class PettyCashExpense : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid PettyCashId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public Guid CostCenterId { get; set; }
        public string? Description { get; set; } = null!;
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int StatusId { get; set; }

        // Navigation Properties
        public Company Company { get; set; }
        public PettyCash PettyCash { get; set; } = null!;
        public ExpenseCategory ExpenseCategory { get; set; } = null!;
        public CostCenter CostCenter { get; set; } = null!;
        public ApplicationUser? ApprovedByUser { get; set; } = null!;
        public PettyCashExpenseStatus Status { get; set; }

    }
}
