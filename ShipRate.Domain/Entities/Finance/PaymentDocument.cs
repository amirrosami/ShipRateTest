using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipRate.Domain.Entities.Finance
{
    public class PaymentDocument : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? BranchId { get; set; }

        public string DocumentNo { get; set; } = default!;
        public int PaymentMethodId { get; set; }
        public int FinanceDirectionId { get; set; }
        public Guid? PartyId { get; set; }

        public long SystemId { get; set; }

        public decimal Amount { get; set; }
        public decimal AllocatedAmount { get; set; }


        public int CurrencyId { get; set; }

        public DateTime DocumentDate { get; set; }
        public DateTime? EffectiveDate { get; set; }

        public Guid? BankId { get; set; }
        public Guid? BankAccountId { get; set; }

        public string? ReferenceNo { get; set; }
        public string? TrackingNo { get; set; }
        public string? Description { get; set; }
        // اگر این سند علی‌الحساب است
        public bool IsAdvancePayment { get; set; }

        //Relations
        public Company Company { get; set; }
        public Branch Branch { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public FinanceDirection FinanceDirection { get; set; }
        public Currency Currency { get; set; }
        public Person? Party { get; set; }
        public Bank? Bank { get; set; }
        public BankAccount? BankAccount { get; set; }
        public Cheque? Cheque { get; set; }
        public ApplicationUser? CreateUser { get; set; }
        public ApplicationUser? EditUser { get; set; }


        public ICollection<SettlementItemPaymentAllocation> SettlementItemAllocations { get; set; }
            = new List<SettlementItemPaymentAllocation>();

        [NotMapped]
        public decimal UnallocatedAmount => Amount - AllocatedAmount;

        [NotMapped]
        public bool IsFullyAllocated => UnallocatedAmount <= 0;
    }
}
