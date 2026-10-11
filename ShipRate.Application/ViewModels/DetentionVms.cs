using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{

    public class CreateDetentionVm
    {
        [Required]
        public int CompanyId { get; set; }
        public Guid PartyId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? TariffBookId { get; set; }
        public Guid? PrevDetentionId { get; set; }

        [Required]
        public int TradeTypeId { get; set; }

        [Required]
        public int FinanceDirectionId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DetentionToDate { get; set; } = DateTime.Today;

        [Required]
        public int MainCurrencyId { get; set; }

        [Required]
        public int FinalCurrencyId { get; set; }
        public decimal FinalVATAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public Guid? ExchangeRateId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ExchangeRateValue { get; set; }

        [StringLength(2000)]
        public string? Remarks { get; set; }
    }
    public class DetentionCalculationResultVm
    {
        public Guid DetentionId { get; set; }

        public decimal GrossAmount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }

        public decimal PayableAmount { get; set; }

        public string MainCurrencyCode { get; set; } = string.Empty;
        public string PaymentCurrencyCode { get; set; } = string.Empty;
        public DateTime DetentionToDate { get; set; }

        public List<DetentionItemListItemVm> Items { get; set; } = [];
    }

    public sealed class DetentionRowVm
    {
        public Guid Id { get; set; }
        public long DetentionNo { get; set; }
        public bool IssuedByUs { get; set; }
        public string? PartyName { get; set; }
        public string? CompanyName { get; set; }
        public string? DirectionName { get; set; }
        public string? TariffTitle { get; set; }
        public string? StatusTitle { get; set; }
        public int StatusId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public string MainCurrencyName { get; set; }

        public decimal FinalPayableAmount { get; set; }
        public string FinalCurrencyName { get; set; }
        public decimal Settled { get; set; }
        public decimal Remain { get; set; }

        public DateTime DetentionToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueDatePersian { get; set; }
        public string CreateUserName { get; set; } = String.Empty;
        public bool IsFinal { get; set; }
        public string? Remarks { get; set; }
        public int FreeDays { get; set; }
    }
    public sealed class DeliveryOrderDetentionsVm
    {
        public Guid DeliveryOrderId { get; set; }
        public Guid BLId { get; set; }
        public string? BLNo { get; set; }
        public List<DetentionRowVm> Items { get; set; } = new();
    }

    public class DetentionSearchVm
    {
        public string? BLNo { get; set; }
        public string? JobFileNo { get; set; }
        public string? BookingNo { get; set; }
        public string? CompanyName { get; set; }
        public string? PartyName { get; set; }
        public int? DetentionStatusId { get; set; }
        public bool? IssuedByUs { get; set; }
        public bool? IsFinal { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool showInactive { get; set; } = false;


        public void Normalize()
        {
            if (Page < 1)
                Page = 1;

            if (PageSize <= 0)
                PageSize = 10;

            if (PageSize > 100)
                PageSize = 100;
        }
    }

    public class DetentionListItemVm
    {
        public Guid Id { get; set; }
        public string BLNo { get; set; }
        public string BookingNo { get; set; }
        public string StatusName { get; set; }
        public string CompanyName { get; set; }
        public string PartyName { get; set; }

        public string FinanceDirectionName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal VATAmount { get; set; }

        public decimal FinalAmount { get; set; }
        public string FinalCurrency { get; set; }

        public decimal SettledAmount { get; set; }
        public decimal RemainAmount { get; set; }
        public decimal? ExchangeRateValue { get; set; }

        public DateTime DetentionToDate { get; set; }
        public bool IsFinal { get; set; }
        public int StatusId { get; set; }

    }

    public class DetentionItemListItemVm
    {
        public Guid Id { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public int FreeDays { get; set; }
        public int LateDays { get; set; }
        public decimal Amount { get; set; }
    }

    public class DetentionItemDetailListItemVm
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public decimal RatePerDay { get; set; }
        public decimal Amount { get; set; }
    }


    public class DetentionStartBaseViewModel
    {
        public int? Id { get; set; }
        public int CompanyId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BookingId { get; set; }

        [Display(Name = "Buy Detention Start Date")]
        [Required(ErrorMessage = "Please enter the buy detention date")]
        [DataType(DataType.Date)]
        public DateTime? DetentionStartBaseDateForBuy { get; set; }

        [Display(Name = "Sell Detention Start Date")]
        [Required(ErrorMessage = "Please enter the sell detention date")]
        [DataType(DataType.Date)]
        public DateTime? DetentionStartBaseDateForSell { get; set; }
    }

    public class DetentionReportMasterDto
    {
        public Guid DetentionId { get; set; }
        public long DetentionNo { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public string? BookingNo { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string IssuerPartyName { get; set; } = string.Empty;
        public string IssuedToPartyName { get; set; } = string.Empty;
        public string TariffBookName { get; set; } = string.Empty;
        public string TradeTypeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public DateTime DetentionToDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal PreviousPaid { get; set; }
        public decimal FinalPayable { get; set; }
        public string FinalCurrencyCode { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string? LogoPath { get; set; }
        public DateTime PrintDate { get; set; } = DateTime.Now;
        public byte[] LogoData { get; set; } // به جای string LogoPath
    }

    public class DetentionReportItemDto
    {
        public string ContainerNumber { get; set; } = string.Empty;
        public string ContainerSizeTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int TotalDays { get; set; }
        public int FreeDays { get; set; }
        public int LateDays { get; set; }
        public decimal Amount { get; set; }
    }

    public class DetentionStatementReportDto
    {
        // Master Info
        public Guid DetentionId { get; set; }
        public string BLNo { get; set; } = "-";
        public string BookingNo { get; set; } = "-";
        public string CompanyName { get; set; } = string.Empty;
        public string IssuerPartyName { get; set; } = "-";
        public string IssuedToPartyName { get; set; } = "-";
        public string TariffBookName { get; set; } = "-";
        public string TradeTypeName { get; set; } = "-";
        public string StatusName { get; set; } = "-";
        public DateTime DetentionToDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal VATAmount { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public string? Remarks { get; set; }
        public byte[]? LogoData { get; set; }
        public DateTime PrintDate { get; set; } = DateTime.Now;

        // Items
        public List<DetentionItemReportDto> Items { get; set; } = new();
    }

    public class DetentionItemReportDto
    {
        public Guid ItemId { get; set; }
        public Guid DetentionId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public string ContainerSizeType { get; set; } = "-";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public int FreeDays { get; set; }
        public int LateDays { get; set; }
        public decimal Amount { get; set; }

        // Tier Details
        public List<DetentionItemDetailReportDto> Details { get; set; } = new();
    }

    public class DetentionItemDetailReportDto
    {
        public Guid DetailId { get; set; }
        public Guid ItemId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public decimal RatePerDay { get; set; }
        public decimal Amount { get; set; }
    }


}
