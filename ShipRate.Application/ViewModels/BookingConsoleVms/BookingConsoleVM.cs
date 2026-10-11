namespace ShipRate.Application.ViewModels.BookingConsoleVms
{
    public sealed class BookingConsoleVm
    {
        public Guid BookingId { get; set; }
        public string BookingNo { get; set; }
        public Guid JobId { get; set; }
        public BookingConsoleBookingSummaryVm Summary { get; set; } = new();
    }
    public class BookingConsoleBookingSummaryVm
    {
        public Guid BookingId { get; set; }
        public Guid JobId { get; set; }
        public string BookingNo { get; set; } = string.Empty;
        public string JobFileNo { get; set; } = string.Empty;
        public string SalesUserName { get; set; } = string.Empty;
        public string? ReferenceNo { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string? VesselName { get; set; }
        public string? VoyageNumber { get; set; }
        public string? POL { get; set; }
        public string? POD { get; set; }
        public DateTime? IssueDate { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public int? PackageCount { get; set; }
        public int ContainerCount { get; set; }
        public string? ShipperText { get; set; }
        public string? BookerClientName { get; set; }

        public List<BookingConsoleContainerVm> Containers { get; set; } = new();
        public List<BookingConsoleCargoVm> Cargos { get; set; } = new();

    }
    public class BookingConsoleContainerVm
    {
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
    }
    public sealed class BookingConsoleCargoVm
    {
        public Guid Id { get; set; }
        public Guid? BookingCargoId { get; set; }

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
    public sealed class BookingConsoleDetentionsVm
    {
        public Guid BookingId { get; set; }
        public string? BookingNo { get; set; }
        public bool ShowInActive { get; set; } = false;
        public List<DetentionRowVm> Items { get; set; } = new();
    }
    public class BookingInvoicesVm
    {
        public Guid JobId { get; set; }
        public Guid BookingId { get; set; }
        public List<InvoiceRowVm> Invoices { get; set; } = new();
        public string? FilterType { get; set; }
    }
}
