using ShipRate.Application.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Finance
{
    public class JobInvoicesFilterVm
    {
        public Guid JobId { get; set; }
    }


    public class LookupItemVm
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }

    public class PagedResultVm<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class InvoiceListFilterVm
    {
        public string? InvoiceNo { get; set; }
        public string? InvoicePNo { get; set; }
        public string? PartyName { get; set; }
        public int? InvoiceTypeId { get; set; }
        public int? InvoiceStatusId { get; set; }
        public int? FinanceDirectionId { get; set; }
        public int? CurrencyId { get; set; }

        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }

        public Guid? JobId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class InvoiceListItemVm
    {
        public Guid Id { get; set; }
        public Guid PartyId { get; set; }
        public int InvoiceModelId { get; set; }
        public int DirectionId { get; set; }
        public string InvoiceModelName { get; set; }
        public string CompanyName { get; set; }


        public int InvoiceSubjectId { get; set; }
        public string InvoiceSubjectName { get; set; } = string.Empty;

        public string InvoiceSubjectTitle { get; set; } = string.Empty;

        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoicePNo { get; set; } = string.Empty;

        public string BLNo { get; set; } = string.Empty;
        public string BookingNo { get; set; } = string.Empty;
        public string JobFileNo { get; set; } = string.Empty;




        public string FNFName { get; set; } = string.Empty;
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string InvoiceTypeName { get; set; } = string.Empty;
        public string InvoiceStatusName { get; set; } = string.Empty;
        public string FinanceDirectionName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsFullySettled { get; set; }


        public DateTime CreateDateTime { get; set; }
        public string? CreateUserName { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string? UpdateUserName { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainPayment { get; set; }
        public int PaymentStatusId { get; set; }

        public int Serial { get; set; }
        public string SerialPrefix { get; set; }
        public string SerialDisplay => Serial <= 0
            ? "-"
            : (string.IsNullOrWhiteSpace(SerialPrefix) ? Serial.ToString() : $"{SerialPrefix} {Serial}");
    }

    public class InvoiceListPageVm
    {
        public InvoiceListFilterVm Filter { get; set; } = new();
        public PagedResultVm<InvoiceListItemVm> Result { get; set; } = new();

        public List<LookupItemVm> InvoiceTypes { get; set; } = new();
        public List<LookupItemVm> InvoiceStatuses { get; set; } = new();
        public List<LookupItemVm> FinanceDirections { get; set; } = new();
        public List<LookupItemVm> Currencies { get; set; } = new();
    }


    public class OnlinePaymentListPageVm
    {
        public OnlinePaymentFilterVm Filter { get; set; } = new();
        public PagedResultVm<OnlinePaymentListVM> Result { get; set; } = new();
    }


    public class InvoiceTotalsVm
    {
        public Guid InvoiceId { get; set; }
        public decimal FirstTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class InvoiceEditVm
    {
        public Guid? Id { get; set; }
        public Guid? Id2 { get; set; }

        public Guid? JobId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }

        public int Serial { get; set; }
        public string SerialPrefix { get; set; } = string.Empty;

        public int? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        public int InvoiceModelId { get; set; }

        [Required]
        public int InvoiceSubjectId { get; set; }

        public string InvoiceSubjectTitle { get; set; } = string.Empty;

        public int? ClearanceTypeId { get; set; }

        [Required]
        public int FinanceDirectionId { get; set; }

        [Required]
        public int FNFId { get; set; }

        [Required]
        public int InvoiceTypeId { get; set; }

        [Required]
        public int InvoiceStatusId { get; set; }

        [StringLength(30)]
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? ParentInvoiceId { get; set; }
        public Guid? DetentionId { get; set; }

        public Guid? PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;

        [Required]
        public int CurrencyId { get; set; }

        public bool IsPreInvoice { get; set; }

        [StringLength(30)]
        public string? InvoicePNo { get; set; } = string.Empty;
        public DateTime? InvoicePDate { get; set; }


        public decimal? ExchangeRate { get; set; }

        [StringLength(200)]
        public string Remarks { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public decimal FirstTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal? DefaultTaxPercent { get; set; }

        public List<LookupItemVm> InvoiceTypes { get; set; } = new();
        public List<LookupItemVm> InvoiceStatuses { get; set; } = new();
        public List<LookupItemVm> FinanceDirections { get; set; } = new();
        public List<LookupItemVm> Currencies { get; set; } = new();
        public List<LookupItemVm> FNFS { get; set; } = new();
        public List<LookupItemVm> Companies { get; set; } = new();
        public List<LookupItemVm> InvoiceSubjects { get; set; } = new();
        public List<LookupItemVm> Jobs { get; set; } = new();
        public List<LookupItemVm> BLs { get; set; } = new();
        public List<LookupItemVm> Bookings { get; set; } = new();
        public List<LookupItemVm> InvoiceModels { get; set; } = new();
        public List<LookupItemVm> Branches { get; set; } = new();
        public List<LookupItemVm> ClearanceTypes { get; set; } = new();
    }

    public class InvoiceItemEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid InvoiceId { get; set; }

        public int SequenceNumber { get; set; }

        [Required]
        public int ChargeTypeId { get; set; }

        public Guid? ChargeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal QTY { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TaxPercent { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [StringLength(50)]
        public string? ChargeTypeSupplementRemarks { get; set; } = string.Empty;

        public decimal AmountBeforeDiscount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<LookupItemVm> ChargeTypes { get; set; } = new();
        public List<LookupItemVm> Currencies { get; set; } = new();
        public List<LookupItemVm> Charges { get; set; } = new();
    }

    public class InvoiceItemRowVm
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public int SequenceNumber { get; set; }
        public string ChargeTypeName { get; set; } = string.Empty;
        public string ChargeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal QTY { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal AmountBeforeDiscount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime CreateDateTime { get; set; }
        public string? CreateUserName { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string? UpdateUserName { get; set; }

    }

    public class PartyLookupFilterVm
    {
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


    public class PartyLookupRowVm
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = string.Empty;
    }


}
