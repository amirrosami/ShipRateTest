using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class Charge : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid JobId { get; set; }

        public Guid? ShipmentId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? BookingLegId { get; set; }
        public Guid? ContainerId { get; set; }
        public Guid? BLOperationId { get; set; }
        public Guid? JobServiceId { get; set; }
        public Guid? ChargeGroupId { get; set; }
        public Guid? ParentChargeId { get; set; }
        public Guid? JobCompanyRoleId { get; set; }

        public int LineNumber { get; set; }

        public int ChargeTypeId { get; set; }
        public int DirectionId { get; set; }
        public string? ChargeDescription { get; set; }
        public int? CalculationById { get; set; }

        public Guid? PartyId { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }
        public int? BaseCurrencyId { get; set; }
        public decimal? BaseAmount { get; set; }

        public bool IsTaxable { get; set; }
        public decimal? TaxPercent { get; set; }
        public decimal? TaxAmount { get; set; }

        public decimal? GrossAmount { get; set; }

        public bool IsApproved { get; set; }
        public Guid? ApprovedById { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public bool IsPostedToAcc { get; set; }
        public DateTime? PostedToAccDate { get; set; }


        public string? SourceType { get; set; }
        public Guid? SourceId { get; set; }

        public string? Remarks { get; set; }

        //Relations
        public Company Company { get; set; }
        public Job Job { get; set; }
        public Booking? Booking { get; set; }
        public BookingLeg? BookingLeg { get; set; }
        public ChargeType ChargeType { get; set; }
        public CalculationBy? CalculationBy { get; set; }
        public FinanceDirection FinanceDirection { get; set; }

        public Person? Party { get; set; }
        public Currency Currency { get; set; }
        public Currency? BaseCurrency { get; set; }
        public ApplicationUser? ApprovedBy { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<SettlementItem> SettlementItems { get; set; } = new List<SettlementItem>();

    }
}
