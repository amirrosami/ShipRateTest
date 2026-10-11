using ShipRate.Domain.Entities.Messaging;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    // ============ Queries / Lists ============
    public sealed class NOAFilterDto : NPagedRequestDto
    {
        public int CompanyId { get; set; }
        public string? Search { get; set; }
        public string? BlNumber { get; set; }
        public Guid? BLId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class NPagedRequestDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class NOAListDto
    {
        public Guid Id { get; set; }
        public string NOANumber { get; set; } = null!;
        public int SequenceNumber { get; set; }
        public int CompanyId { get; set; }
        public int InOutId { get; set; }
        public string CompanyName { get; set; }
        public string InOutName { get; set; }
        public string? BLNumber { get; set; }
        public string? MasterBLNumber { get; set; }
        public DateTime? MasterNOAReceiveDate { get; set; }
        public string? VesselName { get; set; }
        public string? VoyageNo { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? AvailableForPickupDate { get; set; }
        public bool HasMaster { get; set; }
        public string? MasterNOANo { get; set; }
        public string? ConsigneeName { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    // ============ Form ============
    public sealed class NOAFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        public int InOutId { get; set; }

        public string? NOANumber { get; set; }       // رید-آنلی
        public int SequenceNumber { get; set; }      // رید-آنلی

        [Required]
        public int CompanyId { get; set; }

        public bool HasMaster { get; set; }
        public string? MasterNOANo { get; set; }
        public DateTime? MasterNOAReceiveDate { get; set; }

        public Guid? CarrierId { get; set; }
        public Guid? AgentId { get; set; }


        [Required]
        public Guid BLId { get; set; }

        public string? BLNumber { get; set; }        // رید-آنلی
        public Guid? VoyageId { get; set; }
        public string? VesselName { get; set; }
        public string? VoyageNo { get; set; }

        [Required]
        public Guid? ArrivalPortId { get; set; }

        public Guid? DischargeTerminalId { get; set; }
        public DateTime? AvailableForPickupDate { get; set; }

        [Required]
        public Guid? ConsigneeId { get; set; }

        public Guid? NotifyPartyId { get; set; }

        public Guid? NOAFormatId { get; set; }

        public string? SpecialInstructions { get; set; }
        public string? Remarks { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public long CalcRate { get; set; }
    }
    public sealed class NOAFormViewModel
    {
        public NOAFormDto Form { get; set; } = new();
        public ReferenceDataDto? Ref { get; set; }
        public PriceGridVM PriceGrid { get; set; } = new(); // افزودن گرید قیمت‌ها
    }

    // ============ Reference Data ============
    public sealed class ReferenceDataDto
    {
        public List<NLookupDto> Companies { get; set; } = new();
        public List<NLookupDto> Carriers { get; set; } = new();
        public List<NLookupDto> Agents { get; set; } = new();
        public List<NLookupDto> ArrivalPorts { get; set; } = new();
        public List<NLookupDto> DischargeTerminals { get; set; } = new();
        public List<NLookupDto> Persons { get; set; } = new();
        public List<NLookupDto> Formats { get; set; } = new();
        public List<NLookupDto> SendToolTypes { get; set; } = new();
        public List<NLookupDto> InOuts { get; set; } = new();
    }

    public sealed class NLookupDto
    {
        public string Value { get; set; } = null!;   // Guid/int به صورت رشته
        public string Text { get; set; } = null!;
    }

    // ============ Results ============
    public sealed class BLMatchResultDto
    {
        public bool Found { get; set; }
        public bool Multiple { get; set; }
        public string? Message { get; set; }
        public NOAFormDto? Form { get; set; }        // وقتی فقط یک BL پیدا شد
    }

    public sealed class SaveNOAResult
    {
        public bool Success { get; set; }
        public Guid? Id { get; set; }
        public string? NOANumber { get; set; }
        public string? Error { get; set; }
        public string? SpError { get; set; }
        public List<NOAPriceDto>? Prices { get; set; }   // خروجی بعد از SP
    }

    public sealed class NOAPriceDto
    {
        public Guid Id { get; set; }
        public Guid NOAId { get; set; }
        public Guid SequenceNo { get; set; }
        public int ChargeTypeId { get; set; }
        public string? ChargeTypeName { get; set; }
        public decimal InitialValue { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal? SysCalcInitialValue { get; set; }
        public decimal? SysCalcVATAmount { get; set; }
        public decimal? SysCalcTotalValue { get; set; }
        public int CurrencyId { get; set; }
        public int? ClearanceTypeId { get; set; }
        public string? ClearanceTypeName { get; set; }
        public string? CurrencyCode { get; set; }
        public bool IsConditional { get; set; }
        public string? ConditionText { get; set; }
    }

    public sealed class NOASendDto
    {
        public Guid Id { get; set; }
        public int SendToolTypeId { get; set; }
        public string? SendToolTypeName { get; set; }
        public string? Sender { get; set; }
        public string? Receiver { get; set; }
        public DateTime? SendDateTime { get; set; }
        public DateTime? ReceiveDateTime { get; set; }


        // وضعیت اختصاصی پیامک
        public Guid? SMSMessageId { get; set; }
        public SmsDeliveryStatus? SmsStatus { get; set; }
        public string? ErrorDescription { get; set; }


    }

    public sealed class NOASendResultDto
    {
        public bool Success { get; set; }
        public Guid? SendId { get; set; }
        public string? Message { get; set; }
    }

    public sealed class PriceGridVM
    {
        public Guid NOAId { get; set; }
        public string? NOANumber { get; set; }
        public List<NOAPriceDto> Prices { get; set; } = new();
        public string? SpError { get; set; }

        public NOAPriceDto NewRow { get; set; } = new();

        public List<NLookupDto> ChargeTypes { get; set; } = new();
        public List<NLookupDto> ClearanceTypes { get; set; } = new(); // DO.Type

    }
    public sealed class SaveMessageVM
    {
        public string? Error { get; set; }
    }

    public class NOAPrintDto
    {
        public Guid Id { get; set; }
        public string NOANumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;

        // Vessel
        public string VesselName { get; set; } = string.Empty;
        public string VoyageNo { get; set; } = string.Empty;
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public string LoadingPort { get; set; } = string.Empty;
        public string DischargePort { get; set; } = string.Empty;

        // Cargo
        public string BLNumber { get; set; } = string.Empty;
        public int ContainerCount { get; set; }
        public string CargoDescription { get; set; } = string.Empty;
        public DateTime? AvailableForPickupDate { get; set; }

        // Parties
        public string ConsigneeName { get; set; } = string.Empty;
        public string NotifyPartyName { get; set; } = string.Empty;
        public string ConsigneeAddress { get; set; } = string.Empty;

        public DateTime? IssuedDate { get; set; }
    }


    public class NOAPrintViewModel
    {
        // ===== Header =====
        public string NOANumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAbb { get; set; } = string.Empty;
        public byte[]? CompanyLogo { get; set; }

        public DateTime? IssuedDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? AvailableForPickupDate { get; set; }
        public string? MasterNOANo { get; set; }

        // ===== Parties =====
        public string ConsigneeName { get; set; } = string.Empty;
        public string ConsigneeAddress { get; set; } = string.Empty;
        public string ConsigneePhone { get; set; } = string.Empty;

        public string NotifyPartyName { get; set; } = string.Empty;
        public string NotifyPartyAddress { get; set; } = string.Empty;
        public string NotifyPartyPhone { get; set; } = string.Empty;

        public string ShipperName { get; set; } = string.Empty;
        public string ShipperAddress { get; set; } = string.Empty;
        public string ShipperPhone { get; set; } = string.Empty;

        // ===== Cargo =====
        public string BLNumber { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string VoyageNo { get; set; } = string.Empty;
        public string GoodsDescription { get; set; } = string.Empty;
        public string MarksAndNumbers { get; set; } = string.Empty;
        public string PackageTypeText { get; set; } = string.Empty;
        public int TotalPackages { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }
        public int TotalContainers { get; set; }

        public string ArrivalPortName { get; set; } = string.Empty;
        public string DischargeTerminalName { get; set; } = string.Empty;

        public string SpecialInstructions { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;


        public decimal? CashDepositTip { get; set; }
        public decimal? CashDepositStrip { get; set; }
        public decimal? ChequeGuaranteeTip { get; set; }
        public decimal? ChequeGuaranteeStrip { get; set; }


        // ===== Costs =====
        public List<NOAPriceLineViewModel> PriceLines { get; set; } = new();

        // ===== Dynamic free text (closing) =====
        public string ClosingText { get; set; } = string.Empty;

        // ===== Dynamic footer (passed from outside) =====
        public string FooterText { get; set; } = string.Empty;
        public string PageFooterText { get; set; } = string.Empty;

        // ===== Computed =====
        public decimal TotalAmount { get; set; }
        public string CurrencySymbol { get; set; } = string.Empty;
    }

    public class NOAPriceLineViewModel
    {
        public int SequenceNo { get; set; }
        public string ChargeTypeName { get; set; } = string.Empty;
        public string ChargeTypeLocalName { get; set; } = string.Empty;
        public decimal InitialValue { get; set; }
        public decimal VATAmount { get; set; }
        public decimal TotalValue { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string? ConditionText { get; set; }
        public int ClearanceTypeId { get; set; }
    }


    public class NOAPriceMatrixReportDto
    {
        public int SequenceNo { get; set; }
        public string ChargeTypeName { get; set; } = string.Empty;
        public string ChargeTypeLocalName { get; set; } = string.Empty;

        // TIP (ClearanceTypeId = 1)
        public decimal TipInitialValue { get; set; }
        public decimal TipVATAmount { get; set; }
        public decimal TipTotalValue { get; set; }

        // Strip (ClearanceTypeId = 2)
        public decimal StripInitialValue { get; set; }
        public decimal StripVATAmount { get; set; }
        public decimal StripTotalValue { get; set; }
    }


    public class NOASendHubViewModel
    {
        public Guid NOAId { get; set; }
        public string NOANumber { get; set; } = string.Empty;
        public string BLNumber { get; set; } = string.Empty;

        // Presets for Form
        public string DefaultSender { get; set; } = "System SMS";
        public string DefaultMobile { get; set; } = string.Empty;
        public string DefaultEmail { get; set; } = string.Empty;
        public string DefaultPortalUser { get; set; } = string.Empty;

        // History Log
        public List<NOASendDto> History { get; set; } = new();
    }
    public class NOASendRequestDto
    {
        public Guid NOAId { get; set; }
        public int SendToolTypeId { get; set; } // 1: SMS, 2: Email, 3: Portal
        public string Sender { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
    }
}
