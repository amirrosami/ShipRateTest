using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;
using System.ComponentModel.DataAnnotations.Schema;
namespace ShipRate.Domain.Entities.DetentionCalculation
{
    public class Detention : BusinessEntity
    {
        public long DetentionNo { get; set; }
        public int CompanyId { get; set; }
        public int FinanceDirectionId { get; set; }
        public Guid? PrevDetentionId { get; set; }
        public Guid? PartyId { get; set; }

        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? TariffBookId { get; set; }

        public int TradeTypeId { get; set; }
        public int DetentionStatusId { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal VATPercent { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public int MainCurrencyId { get; set; }

        public DateTime DetentionToDate { get; set; }
        public int DetentionUpdateById { get; set; }

        public Guid? ExchangeRateId { get; set; }
        public decimal? ExchangeRateValue { get; set; }

        public int FinalCurrencyId { get; set; }
        public decimal PreviousPaidAmount { get; set; }

        public decimal FinalVATAmount { get; set; }
        public decimal FinalPayableAmount { get; set; }


        public decimal SettledAmount { get; set; }
        [NotMapped]
        public decimal RemainingAmount => FinalPayableAmount - SettledAmount;

        [NotMapped]
        public bool IsFullySettled => RemainingAmount <= 0;


        public bool IsFinal { get; set; }
        public string? Remarks { get; set; }

        //Relations

        public Company Company { get; set; }
        public Person Party { get; set; }
        public FinanceDirection FinanceDirection { get; set; }
        public Currency? MainCurrency { get; set; }
        public Currency? PaymentCurrency { get; set; }
        public Detention? PrevDetention { get; set; }

        public BL? BL { get; set; }
        public Booking? Booking { get; set; }
        public ExchangeRate ExchangeRate { get; set; }
        public TradeType TradeType { get; set; }
        public DetentionStatus DetentionStatus { get; set; }
        public DetentionUpdater DetentionUpdater { get; set; }
        public TariffBook TariffBook { get; set; }

        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser EditUser { get; set; }



        public ICollection<DetentionItem> Items { get; set; } = new List<DetentionItem>();
        public ICollection<SettlementItem> SettlementItems { get; set; } = new List<SettlementItem>();

    }
    public class DetentionItem : BusinessEntity
    {
        public Guid DetentionId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public Guid? ContainerId { get; set; }

        public string ContainerNumber { get; set; } = string.Empty;
        public int ContainerSizeTypeId { get; set; }

        public DateTime DetentionItemStartDate { get; set; }
        public DateTime DetentionItemEndDate { get; set; }
        public bool IsReturned { get; set; } = false;
        public DateTime? DetentionItemReturnDate { get; set; }
        public int DetentionItemTotalDays { get; set; }
        public int DetentionItemFreeDays { get; set; }
        public int DetentionItemLateDays { get; set; }
        public decimal DetentionItemAmount { get; set; }

        //Relations
        public Detention Detention { get; set; }
        public ContainerUse ContainerUse { get; set; }
        public Container Container { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }

        public ICollection<DetentionItemDetail> Details { get; set; }
    }

    public class DetentionItemDetail : BusinessEntity
    {
        public Guid DetentionItemId { get; set; }
        public DateTime DetentionItemDetailFromDate { get; set; }
        public DateTime DetentionItemDetailToDate { get; set; }
        public int DetentionItemDetailTotalDays { get; set; }
        public decimal DetentionItemDetailRatePerDay { get; set; }
        public decimal DetentionItemDetailAmount { get; set; }

        //Relations
        public DetentionItem DetentionItem { get; set; }
    }



    public class DetentionStartBase : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }
        public DateTime DetentionStartBaseDateForBuy { get; set; }
        public DateTime DetentionStartBaseDateForSell { get; set; }

        //Relations
        public Company Company { get; set; }
        public BL BL { get; set; }
        public Booking Booking { get; set; }
    }
    public class OnlineDetention : BusinessEntity
    {
        public int CompanyId { get; set; }
        public bool IssuedByUs { get; set; }
        public Guid? IssuerPartyId { get; set; }
        public Guid? IssuedToPartyId { get; set; }

        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }

        public decimal OnlineDetentionGross { get; set; }
        public decimal OnlineDetentionVat { get; set; }
        public decimal OnlineDetentionNet { get; set; }

        public int StatusId { get; set; }

        //Relations
        public Company Company { get; set; }
        public Person IssuerParty { get; set; }
        public Person IssuedToParty { get; set; }
        public BL? BL { get; set; }
        public Booking? Booking { get; set; }
        public DetentionStatus? Status { get; set; }
    }
}