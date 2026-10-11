using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.DetentionCalculation;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class Settlement : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int SettlementNo { get; set; }
        public DateTime SettlementDate { get; set; }
        public Guid? PartyId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }
        // ارز پایه گزارش سند، نه لزوماً ارز همه آیتم‌ها
        public int CurrencyId { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public int StatusId { get; set; }
        //Relations
        public Company Company { get; set; }
        public Person Party { get; set; }
        public Currency Currency { get; set; }
        public SettlementStatus Status { get; set; }
        public BL BL { get; set; }
        public Booking Booking { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser EditUser { get; set; }

        public virtual ICollection<SettlementItem> Items { get; set; }
               = new List<SettlementItem>();

        public virtual ICollection<SettlementItemPaymentAllocation> PaymentAllocations { get; set; }
        = new List<SettlementItemPaymentAllocation>();
    }
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class SettlementItem : BusinessEntity
    {
        public Guid SettlementId { get; set; }
        public Guid? InvoiceId { get; set; }
        public Guid? InvoiceItemId { get; set; }
        public Guid? ChargeId { get; set; }
        public Guid? DetentionId { get; set; }

        public decimal Amount { get; set; }
        public decimal? AmountInBaseCurrency { get; set; }
        public decimal? BaseExchangeRate { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        //Relations
        public Settlement Settlement { get; set; }
        public Invoice Invoice { get; set; }
        public InvoiceItem InvoiceItem { get; set; }
        public Detention Detention { get; set; }
        public Charge Charge { get; set; }

        public virtual ICollection<SettlementItemPaymentAllocation> PaymentAllocations { get; set; }
              = new List<SettlementItemPaymentAllocation>();


        public decimal CoveredAmountInItemCurrency =>
                PaymentAllocations?.Sum(x => x.SettledAmountInItemCurrency) ?? 0m;

        public decimal RemainingAmountInItemCurrency =>
              Amount - CoveredAmountInItemCurrency;
    }
    //********************************************************************************************************************************************************************************
    //********************************************************************************************************************************************************************************
    public class SettlementItemPaymentAllocation : BusinessEntity
    {
        public Guid SettlementId { get; set; }
        public Guid SettlementItemId { get; set; }
        public Guid PaymentDocumentId { get; set; }

        // مبلغی که از ابزار پرداخت مصرف شده
        public decimal AllocatedAmount { get; set; }

        // مقدار پوشش داده شده از آیتم در ارز خود آیتم
        public decimal SettledAmountInItemCurrency { get; set; }

        // نرخ تبدیل AllocationCurrency -> ItemCurrency
        public Guid? ExchangeRateId { get; set; }
        public int? ExchangeRateBaseId { get; set; }
        public DateTime? ExchangeRateDate { get; set; }
        public decimal ExchangeRateValue { get; set; }

        // برای گزارش‌گیری در ارز پایه Settlement
        public decimal? AllocatedAmountInBaseCurrency { get; set; }

        public string? Remarks { get; set; }

        //Relations
        public virtual Settlement Settlement { get; set; } = default!;
        public virtual SettlementItem SettlementItem { get; set; } = default!;
        public virtual PaymentDocument PaymentDocument { get; set; } = default!;
        public virtual ExchangeRate? ExchangeRateEntity { get; set; }
        public ExchangeRateBase? ExchangeRateBase { get; set; }

    }








}
