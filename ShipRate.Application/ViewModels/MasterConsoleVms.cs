namespace ShipRate.Application.ViewModels
{

    public sealed class MasterConsoleVm
    {
        public Guid BLId { get; set; }
        public Guid JobId { get; set; }
        public MasterConsoleBLSummaryVm Summary { get; set; } = new();
        public MasterConsoleStatusSummaryVm StatusSummary { get; set; } = new();
    }
    public class MasterConsoleBLSummaryVm
    {
        public Guid BLId { get; set; }
        public Guid JobId { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public string JobFileNo { get; set; } = string.Empty;
        public string SalesUserName { get; set; } = string.Empty;
        public string? ReferenceNo { get; set; }
        public string? BookingNo { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string? VesselName { get; set; }
        public string? VoyageNumber { get; set; }
        public string? POL { get; set; }
        public string? POD { get; set; }
        public DateTime? IssueDate { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public int? PackageCount { get; set; }
        public int ContainerCount { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public string? ConsigneeText { get; set; }
        public string? NotifyText { get; set; }
        public string? BookerClientName { get; set; }

        public List<MasterConsoleContainerVm> Containers { get; set; } = new();
        public List<MasterConsoleCargoVm> Cargos { get; set; } = new();

    }
    public class MasterConsoleContainerVm
    {
        public Guid DeliveryOrderContainerId { get; set; }
        public Guid BLContainerId { get; set; }
        public string ContainerNo { get; set; } = string.Empty;
        public string SealNo { get; set; } = string.Empty;
        public string? ContainerType { get; set; }
        public string? ContainerSize { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public int? PackageCount { get; set; }

        public Guid? ContainerUseId { get; set; }
        public DateTime? FirstLOBDate { get; set; }
        public DateTime? LastDischargeDate { get; set; }
        public DateTime? RTSDate { get; set; }
        public DateTime? RBSDate { get; set; }
        public DateTime? DTCDate { get; set; }
        public DateTime? RBCDate { get; set; }
    }
    public sealed class MasterConsoleCargoVm
    {
        public Guid Id { get; set; }
        public Guid DeliveryOrderId { get; set; }
        public Guid? BLCargoId { get; set; }

        public string? PackageTypeName { get; set; }
        public string? HSCode { get; set; }
        public decimal? Volume { get; set; }
        public decimal? PackageCount { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public string? Measurement { get; set; }
        public string? MarksAndNumbers { get; set; }
        public string? DescriptionOfGoods { get; set; }
    }
    public sealed class MasterConsoleDetentionsVm
    {
        public Guid BLId { get; set; }
        public string? BLNo { get; set; }
        public bool ShowInActive { get; set; } = false;
        public List<DetentionRowVm> Items { get; set; } = new();
    }

    public class MasterConsoleStatusSummaryVm
    {
        // وضعیت کلی بارنامه
        public string BLStatus { get; set; } = "Issued"; // Draft, Issued, Void

        // اطلاعات آزادسازی (Release)
        public string ReleaseType { get; set; } // Telex Release, Original BL, Waybill, ...
        public DateTime? ReleaseDate { get; set; }
        public bool IsReleased => ReleaseDate.HasValue;

        // وضعیت Delivery Order (ترخیصیه)
        public bool IsDOIssued { get; set; }
        public string? DONumber { get; set; }
        public DateTime? DODate { get; set; }

        // وضعیت NOA (اعلامیه ورود)
        public bool IsNOAIssued { get; set; }
        public DateTime? NOADate { get; set; }

        // وضعیت مالی / تسویه (برای اجازه ترخیص)
        public bool IsFinancialCleared { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
    }










    public class DoPricePreviewItemDto
    {
        public int Seq { get; set; }
        public int ChargeTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AmountBeforeDiscount { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal TaxRatePercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DoPricePreviewViewModel
    {
        public Guid BLId { get; set; }
        public int ClearanceTypeId { get; set; } = 1;
        public decimal TaxRate { get; set; } = 10.00m;
        public List<DoPricePreviewItemDto> Items { get; set; } = new();
    }
}
