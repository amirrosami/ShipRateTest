namespace ShipRate.Application.DTOs.BillOfLadings
{
    public enum TransportTypeEnum
    {
        Sea = 1,
        Road = 2,
        Rail = 3,
        Air = 4,
        MultiModal = 5
    }

    public enum BLManagementOperationalStatusEnum
    {
        Unknown = 0,
        Draft = 1,
        InProgress = 2,
        Ready = 3,
        Released = 4,
        Delivered = 5,
        Closed = 6,
        Cancelled = 7,
        Delayed = 8
    }

    public enum BLManagementHealthStatusEnum
    {
        Unknown = 0,
        Complete = 1,
        MissingData = 2,
        Attention = 3
    }

    public enum BLManagementSortFieldEnum
    {
        IssueDate = 1,
        BLNo = 2,
        JobNo = 3,
        CustomerName = 4,
        ShipperName = 5,
        ConsigneeName = 6,
        TransportType = 7,
        DocumentStatus = 8,
        OperationalStatus = 9,
        CreatedDate = 10,
        ModifiedDate = 11
    }


    public class BLManagementSearchRequest
    {
        public string? GlobalSearch { get; set; }
        public int? TransportTypeId { get; set; }
        public int? BLTypeId { get; set; }
        public int? BLStatusId { get; set; }
        public string? BLNo { get; set; }
        public string? JobNo { get; set; }
        public string? CustomerName { get; set; }
        public string? ShipperName { get; set; }
        public string? ConsigneeName { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? IssueDateFrom { get; set; }
        public DateTime? IssueDateTo { get; set; }
        public string? ContainerNo { get; set; }
        public string? VesselVoyage { get; set; }
        public string? FlightNo { get; set; }
        public string? TruckNo { get; set; }
        public string? TrainNo { get; set; }
        public string? WagonNo { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class BLManagementRowDto
    {
        public Guid BLId { get; set; }
        public Guid JobId { get; set; }
        public string BLNo { get; set; } = string.Empty;
        public string JobNo { get; set; } = string.Empty;
        public string JobFileNumber { get; set; } = string.Empty;
        public string SalesUserName { get; set; } = string.Empty;
        public int? TradeTypeId { get; set; }
        public string? TradeTypeName { get; set; }
        public int TransportTypeId { get; set; }
        public string TransportTypeTitle { get; set; } = string.Empty;
        public string BLTypeTitle { get; set; } = string.Empty;
        public string DocumentStatusTitle { get; set; } = string.Empty;
        public string ShipperName { get; set; } = string.Empty;
        public string ConsigneeName { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string RouteText { get; set; } = string.Empty;
        public decimal? GrossWeightKg { get; set; }
        public DateTime? IssueDate { get; set; }
        public string ModeDetailTitle { get; set; } = string.Empty;
        public string ModeDetailSubTitle { get; set; } = string.Empty;
        public bool IsDeleted => BLDeletedAt.HasValue;
        public DateTime? BLDeletedAt { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanEdit { get; set; }
        public bool CanPrint { get; set; }
        public bool CanRelease { get; set; }
        public bool CanCancel { get; set; }
        public bool CanDelete { get; set; }
    }

    public class BLManagementRowActionsDto
    {
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanPrint { get; set; }
        public bool CanRelease { get; set; }
        public bool CanCancel { get; set; }
        public bool CanDelete { get; set; }
        public bool CanRestore { get; set; }
    }

    public class BLManagementSummaryDto
    {
        public int TotalBills { get; set; }
        public int InProgressCount { get; set; }
        public int IncompleteCount { get; set; }
        public int MissingCarrierReferenceCount { get; set; }
    }
    public class BLManagementActionResultDto
    {
        public bool Success { get; set; }
        public long BLId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class BLManagementPageViewModel
    {
        public BLManagementSearchRequest Search { get; set; } = new();
        public BLManagementSummaryDto Summary { get; set; } = new();
        public PagedResultDto<BLManagementRowDto> PagedResult { get; set; } = new();
    }


    public class BLManagementQuickFilterDto
    {
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsActive { get; set; }
    }

    public class BLManagementSearchResponse
    {
        public List<BLManagementRowDto> Items { get; set; } = new();
        public int TotalCount { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }

        public BLManagementSummaryDto Summary { get; set; } = new();
        public List<BLManagementQuickFilterDto> QuickFilters { get; set; } = new();
    }

}
