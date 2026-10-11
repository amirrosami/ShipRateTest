using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Finance
{
    public class SettlementSearchDto
    {
        public int? SettlementNo { get; set; }

        public DateTime? SettlementDateFrom { get; set; }
        public DateTime? SettlementDateTo { get; set; }

        public Guid? PartyId { get; set; }
        public string? PartyName { get; set; }

        public int? StatusId { get; set; }

        public string? InvoiceNo { get; set; }
        public string? PaymentDocumentNo { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }


    public class SettlementListItemDto
    {
        public Guid Id { get; set; }

        public int SettlementNo { get; set; }

        public DateTime SettlementDate { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = "";

        public Guid? PartyId { get; set; }
        public string PartyName { get; set; } = "";

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = "";

        public int StatusId { get; set; }
        public string StatusName { get; set; } = "";

        public decimal TotalSettlementAmount { get; set; }

        public decimal TotalAllocatedAmount { get; set; }

        public DateTime CreateDate { get; set; }
        public string CreateUserName { get; set; } = string.Empty;

        public string Remarks { get; set; } = "";
    }


    public class SettlementHeaderCreateDto
    {
        [Required]
        public int CompanyId { get; set; }

        [Required]
        public DateTime SettlementDate { get; set; } = DateTime.Today;

        public Guid? PartyId { get; set; }

        public string? PartyName { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [MaxLength(200)]
        public string Remarks { get; set; } = "";
    }

    public class SettlementHeaderEditDto
    {
        [Required]
        public Guid Id { get; set; }

        public int SettlementNo { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public DateTime SettlementDate { get; set; }

        public Guid? PartyId { get; set; }

        public string? PartyName { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [MaxLength(200)]
        public string Remarks { get; set; } = "";
    }
    public class SettlementDetailsDto
    {
        public SettlementHeaderEditDto Header { get; set; } = new();

        public List<SettlementItemDto> Items { get; set; } = new();

        public List<PaymentAllocationDto> PaymentAllocations { get; set; } = new();
    }
    public class SettlementItemDto
    {
        public Guid Id { get; set; }

        public Guid SettlementId { get; set; }

        public Guid? InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }

        public Guid? InvoiceItemId { get; set; }

        public Guid? ChargeId { get; set; }
        public string? ChargeDescription { get; set; }

        public decimal Amount { get; set; }

        public decimal? AmountInBaseCurrency { get; set; }

        public decimal? BaseExchangeRate { get; set; }

        public string Remarks { get; set; } = "";
    }
    public class SettlementItemCreateDto
    {
        [Required]
        public Guid SettlementId { get; set; }

        public Guid? InvoiceId { get; set; }

        public Guid? InvoiceItemId { get; set; }

        public Guid? ChargeId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public decimal? AmountInBaseCurrency { get; set; }

        public decimal? BaseExchangeRate { get; set; }

        [MaxLength(100)]
        public string Remarks { get; set; } = "";
    }
    public class PaymentAllocationCreateDto
    {
        [Required]
        public Guid SettlementId { get; set; }

        [Required]
        public Guid SettlementItemId { get; set; }

        [Required]
        public Guid PaymentDocumentId { get; set; }

        [Required]
        public decimal AllocatedAmount { get; set; }

        [Required]
        public decimal SettledAmountInItemCurrency { get; set; }

        public Guid? ExchangeRateId { get; set; }

        public int? ExchangeRateBaseId { get; set; }

        public DateTime? ExchangeRateDate { get; set; }

        [Required]
        public decimal ExchangeRateValue { get; set; } = 1;

        public decimal? AllocatedAmountInBaseCurrency { get; set; }

        [MaxLength(100)]
        public string Remarks { get; set; } = "";
    }
    public class PaymentAllocationDto
    {
        public Guid Id { get; set; }

        public Guid SettlementId { get; set; }

        public Guid SettlementItemId { get; set; }

        public Guid PaymentDocumentId { get; set; }

        public string PaymentDocumentNo { get; set; } = "";

        public DateTime PaymentDocumentDate { get; set; }

        public decimal PaymentDocumentAmount { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal SettledAmountInItemCurrency { get; set; }

        public decimal ExchangeRateValue { get; set; }

        public decimal? AllocatedAmountInBaseCurrency { get; set; }

        public string Remarks { get; set; } = "";
    }
    public class LookupDto
    {
        public string Id { get; set; } = "";

        public string Text { get; set; } = "";
    }
    public class PaymentAllocationFormDto
    {
        public Guid SettlementId { get; set; }

        public List<SettlementItemLookupDto> SettlementItems { get; set; } = new();

        public PaymentAllocationCreateDto Input { get; set; } = new();
    }
    public class SettlementItemLookupDto
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public decimal ItemAmount { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal RemainingAmount { get; set; }
    }
    public class JobSettlementGridRowDto
    {
        public Guid SettlementItemId { get; set; }

        public Guid SettlementId { get; set; }

        public string SettlementNo { get; set; }

        public string InvoiceNo { get; set; }

        public string ChargeName { get; set; }

        public string PaymentDocumentNo { get; set; }

        public decimal SettledAmount { get; set; }
    }
    public class JobSettlementDetailDto
    {
        public Guid SettlementId { get; set; }

        public Guid SettlementItemId { get; set; }

        public string SettlementNo { get; set; }

        public DateTime SettlementDate { get; set; }

        public string InvoiceNo { get; set; }

        public string ChargeName { get; set; }

        public decimal ItemAmount { get; set; }

        public string PaymentDocumentNo { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal SettledAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        public string Remarks { get; set; }

        public string ItemType { get; set; }
        public string ItemTitle { get; set; }



    }

    public class OperationResult
    {
        public bool IsSuccess { get; private set; }
        public string? Message { get; private set; }
        public string? ErrorMessage { get; private set; }

        public static OperationResult Success(string message = "") =>
            new() { IsSuccess = true, Message = message };

        public static OperationResult Failure(string errorMessage) =>
            new() { IsSuccess = false, ErrorMessage = errorMessage };
    }


    public class SettlementReceiptReportDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogoPath { get; set; }
        public int SettlementNo { get; set; }
        public string SettlementDate { get; set; } = string.Empty;
        public string PartyName { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal RemainingAmount { get; set; }

        public List<SettlementReceiptItemDto> Items { get; set; } = new();
        public List<SettlementReceiptPaymentDto> Payments { get; set; } = new();
    }

    public class SettlementReceiptItemDto
    {
        public int RowIndex { get; set; }
        public string InvoiceNo { get; set; } = "-";
        public string ChargeTitle { get; set; } = "-";
        public decimal Amount { get; set; }
        public decimal CoveredAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class SettlementReceiptPaymentDto
    {
        public int RowIndex { get; set; }
        public string DocumentNo { get; set; } = "-";
        public string DocumentDate { get; set; } = "-";
        public string PaymentMethod { get; set; } = "-";
        public decimal AllocatedAmount { get; set; }
        public decimal SettledInItemCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }


    public class SettlementModalViewModel
    {
        // هدر تسویه
        public Guid BLId { get; set; }
        public string BLNo { get; set; } = string.Empty;

        public Guid BookingId { get; set; }
        public string BookingNo { get; set; } = string.Empty;

        public Guid? PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public int CurrencyId { get; set; }
        public int CompanyId { get; set; }
        public DateTime SettlementDate { get; set; } = DateTime.Today;
        public string Remarks { get; set; } = string.Empty;

        // آیتم‌های قابل تسویه بارنامه (سمت چپ)
        public List<PendingSettlementItemDto> AvailableItems { get; set; } = new();

        // اسناد پرداخت/دریافت دارای مانده همان طرف‌حساب (یا بارنامه)
        public List<AvailablePaymentDocDto> AvailablePaymentDocuments { get; set; } = new();

        public SelectList Currencies { get; set; }
        public SelectList Parties { get; set; }
        public SelectList Companies { get; set; }
    }

    public class PendingSettlementItemDto
    {
        public Guid SourceId { get; set; } // InvoiceId or DetentionId
        public string ItemType { get; set; } // "Invoice" | "Detention"
        public string DocumentNo { get; set; } // Invoice No or Detention Ref
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public int CompanyId { get; set; }
    }

    public class AvailablePaymentDocDto
    {
        public Guid PaymentDocumentId { get; set; }
        public string DocumentNo { get; set; }
        public DateTime DocumentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal UnallocatedAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string PaymentMethodName { get; set; }
    }

    // مدل ورودی برای متد Save در Controller
    public class SaveSettlementRequestDto
    {
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PartyId { get; set; }
        public int CurrencyId { get; set; }
        public int CompanyId { get; set; }
        public DateTime SettlementDate { get; set; }
        public string Remarks { get; set; }

        public List<SettlementAllocationEntryDto> Allocations { get; set; } = new();
    }

    public class SettlementAllocationEntryDto
    {
        public string ItemType { get; set; } // "Invoice" | "Detention"
        public Guid SourceId { get; set; } // InvoiceId or DetentionId
        public Guid PaymentDocumentId { get; set; }
        public decimal SettledAmountInItemCurrency { get; set; }
        public decimal AllocatedAmountFromDoc { get; set; }
        public decimal ExchangeRateValue { get; set; } = 1.0m;
    }

    public class OpenSettlementModalRequest
    {
        public List<Guid> InvoiceIds { get; set; } = new();
        public List<Guid> DetentionIds { get; set; } = new();

        // در صورت وجود context خاص (اختیاری)
        public Guid? BLId { get; set; }
        public Guid? PartyId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class InvoiceSettlementDto
    {
        public Guid Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public Guid PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
    }


}
