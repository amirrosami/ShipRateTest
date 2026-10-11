using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    public class SLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class SPersonLookupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ServiceRequestListItemDto
    {
        public Guid Id { get; set; }
        public int RequestTypeId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string InOutName { get; set; } = string.Empty;
        public string RequestNo { get; set; } = string.Empty;
        public string RequestTypeName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public string PartyName { get; set; } = string.Empty;
        public string PartyNameDetail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsChargeable { get; set; }


        public DateTime? CreatedAt { get; set; }
        public string? CreateUserName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdateUserName { get; set; }

    }

    public class ServiceRequestSearchDto
    {
        public string? RequestNo { get; set; }
        public string? Subject { get; set; }
        public string? RequesterName { get; set; }
        public int? RequestTypeId { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public Guid PartyId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ServiceRequestCreateUpdateDto
    {
        public Guid? Id { get; set; }

        public int CompanyId { get; set; }

        public int InOutId { get; set; }

        [Required]
        [StringLength(50)]
        public string RequestNo { get; set; } = string.Empty;

        [Required]
        public int RequestTypeId { get; set; }

        [Required]
        public int StatusId { get; set; }

        [Required]
        public int PriorityId { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid? PartyId { get; set; }
        [StringLength(100)]
        public string? PartyDetailName { get; set; }

        [StringLength(100)]
        public string? ExternalReferenceNo { get; set; }

        public Guid? ParentServiceRequestId { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        public bool IsChargeable { get; set; }
        public decimal? EstimatedCostAmount { get; set; }
        public decimal? FinalCostAmount { get; set; }
        public int? CostCurrencyId { get; set; }

        [StringLength(1000)]
        public string? Remarks { get; set; }
    }
    public class ServiceRequestFormDto
    {
        public ServiceRequestCreateUpdateDto Model { get; set; } = new();

        public List<SLookupDto> RequestTypes { get; set; } = new();
        public List<SLookupDto> Statuses { get; set; } = new();
        public List<SLookupDto> Priorities { get; set; } = new();
        public List<SLookupDto> RoleTypes { get; set; } = new();
        public List<SLookupDto> Currencies { get; set; } = new();
        public List<SLookupDto> Companies { get; set; } = new();
        public List<SLookupDto> InOuts { get; set; } = new();
        public string? PartyDisplayName { get; set; }
    }

    public class ServiceRequestIndexDto
    {
        public ServiceRequestSearchDto Search { get; set; } = new();
        public PagedResultDto<ServiceRequestListItemDto> Result { get; set; } = default!;

        public List<SLookupDto> RequestTypes { get; set; } = new();
        public List<SLookupDto> Statuses { get; set; } = new();
        public List<SLookupDto> Priorities { get; set; } = new();
    }



    public class ServiceRequestDto
    {
        public ServiceRequestFormDto ServiceRequestFormDto{ get; set; }
        public object DetailDto { get; set; }
        public string PartialName { get; set; }
    }
    public class ServiceRequestApprovalListItemDto
    {
        public Guid Id { get; set; }

        public Guid ServiceRequestId { get; set; }

        public string? ApprovalTypeName { get; set; }

        public string? StatusName { get; set; }

        public string? ApproverUserName { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? Remarks { get; set; }
    }
    public class ServiceRequestApprovalsPanelDto
    {
        public Guid ServiceRequestId { get; set; }

        public string? ServiceRequestNo { get; set; }

        public List<ServiceRequestApprovalListItemDto> Items { get; set; } = new();
    }
    public class ServiceRequestApprovalFormDto
    {
        public Guid? Id { get; set; }

        public Guid ServiceRequestId { get; set; }

        public int ApprovalTypeId { get; set; }

        public int ApprovalStatusId { get; set; }

        public Guid? ApproverUserId { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? Remarks { get; set; }

        public List<SelectListItem> ApprovalTypes { get; set; } = new();

        public List<SelectListItem> ApprovalStatuses { get; set; } = new();

        public List<SelectListItem> ApproverUsers { get; set; } = new();
    }

    public class ServiceRequestChargeRowDto
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }

        public int ChargeTypeId { get; set; }
        public string? ChargeTypeName { get; set; }

        public string? CostTitle { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        public string? CurrencyName { get; set; }

        public bool IsPayable { get; set; }
        public bool IsReceivable { get; set; }
        public bool IsApproved { get; set; }

        public string? Remarks { get; set; }
    }
    public class ServiceRequestChargesPanelDto
    {
        public Guid ServiceRequestId { get; set; }
        public string? ServiceRequestNo { get; set; }

        public List<ServiceRequestChargeRowDto> Charges { get; set; } = new();
    }


    public class ServiceRequestChargeFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid ServiceRequestId { get; set; }

        [Required]
        [Display(Name = "Charge Type")]
        public int? ChargeTypeId { get; set; }

        [Display(Name = "Cost Title")]
        [StringLength(100)]
        public string? CostTitle { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal? Amount { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public int? CurrencyId { get; set; }

        [Display(Name = "Payable")]
        public bool IsPayable { get; set; }

        [Display(Name = "Receivable")]
        public bool IsReceivable { get; set; }

        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(1000)]
        public string? Remarks { get; set; }

        public List<SelectListItem> ChargeTypes { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
    }


    public class BLAmendmentRequestLineRowDto
    {
        public Guid Id { get; set; }

        public Guid BLAmendmentRequestId { get; set; }

        public int FieldTypeId { get; set; }

        public string? FieldTypeName { get; set; }

        public string? CurrentValue { get; set; }

        public string? RequestedValue { get; set; }

        public string? Remarks { get; set; }
    }


    public class BLAmendmentRequestRowDto
    {
        public Guid Id { get; set; }

        public Guid ServiceRequestId { get; set; }

        public Guid BLId { get; set; }

        public string? BLNumber { get; set; }

        public string? Reason { get; set; }

        public bool RequiresCarrierApproval { get; set; }

        public bool RequiresCustomerApproval { get; set; }

        public List<BLAmendmentRequestLineRowDto> Lines { get; set; } = new();
    }
    public class BLAmendmentsPanelDto
    {
        public Guid ServiceRequestId { get; set; }
        public string? ServiceRequestNo { get; set; }

        public List<BLAmendmentRequestRowDto> Requests { get; set; } = new();
    }

    public class BLAmendmentRequestFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid ServiceRequestId { get; set; }

        [Required]
        [Display(Name = "B/L")]
        public Guid? BLId { get; set; }
        public string? BLNo { get; set; }

        [Display(Name = "Reason")]
        [StringLength(1000)]
        public string? Reason { get; set; }

        [Display(Name = "Requires Carrier Approval")]
        public bool RequiresCarrierApproval { get; set; }

        [Display(Name = "Requires Customer Approval")]
        public bool RequiresCustomerApproval { get; set; }

        public List<BLAmendmentRequestLineFormDto> Lines { get; set; } = new();

        public List<SelectListItem> BLs { get; set; } = new();
    }

    public class BLAmendmentRequestLineFormDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid BLAmendmentRequestId { get; set; }

        [Required]
        public Guid ServiceRequestId { get; set; }

        [Required]
        [Display(Name = "Field Type")]
        public int? FieldTypeId { get; set; }

        [Display(Name = "Current Value")]
        [StringLength(1000)]
        public string? CurrentValue { get; set; }

        [Display(Name = "Requested Value")]
        [StringLength(1000)]
        public string? RequestedValue { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(1000)]
        public string? Remarks { get; set; }
        public int Index { get; set; }
        public List<SelectListItem> FieldTypes { get; set; } = new();
    }


    public class SplitRequestPanelDto
    {
        public Guid ServiceRequestId { get; set; }
        public string RequestNo { get; set; } = null!;
        public int RequestTypeId { get; set; }

        public Guid? SplitRequestId { get; set; }
        public Guid? SourceJobId { get; set; }
        public Guid? SourceBookingId { get; set; }
        public Guid? SourceBLId { get; set; }

        public string? SourceBLNo { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public bool IsFinalized { get; set; }

        public List<SplitRequestTargetDto> Targets { get; set; } = new();
        public List<SplitBillOfLadingOptionDto> AvailableBillsOfLading { get; set; } = new();


    }
    public class SplitBillOfLadingOptionDto
    {
        public Guid Id { get; set; }
        public string BLNo { get; set; } = string.Empty;
    }

    public class SplitRequestTargetDto
    {
        public Guid Id { get; set; }
        public string TargetKey { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? TargetBLNo { get; set; }
        public string? Remarks { get; set; }
        public int SortOrder { get; set; }

        public List<SplitRequestAllocationDto> Allocations { get; set; } = new();
    }

    public class SplitRequestAllocationDto
    {
        public Guid Id { get; set; }
        public string SourceLineKey { get; set; } = null!;
        public int SourceLineType { get; set; }

        public Guid? SourceAllocationId { get; set; }
        public Guid? SourceCargoDetailId { get; set; }

        public string? ContainerNo { get; set; }
        public string? CargoNo { get; set; }
        public string? Description { get; set; }

        public decimal Packages { get; set; }
        public decimal Weight { get; set; }
        public decimal Volume { get; set; }

        public string? Remarks { get; set; }
    }


    public class CreateSplitRequestVm
    {
        public Guid ServiceRequestId { get; set; }

        [Required]
        public Guid SourceBLId { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }
    public class SplitRequestDto
    {
        public Guid ServiceRequestId { get; set; }

        public Guid? SourceJobId { get; set; }
        public Guid? SourceBookingId { get; set; }
        public Guid? SourceBLId { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }

        public bool IsFinalized { get; set; }
    }

    public class ConsolidationRequestDto
    {
        public Guid Id { get; set; }
        public Guid ServiceRequestId { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
    }

    public class BLAmendmentFullRequestFormDto
    {
        public ServiceRequestFormDto Request { get; set; } = new();

        public BLAmendmentRequestFormDto Amendment { get; set; } = new();

        public List<BLAmendmentRequestLineFormDto> Lines { get; set; } = new();

        public List<SLookupDto> RequestTypes { get; set; } = new();
        public List<SLookupDto> Companies { get; set; } = new();
        public List<SLookupDto> InOuts { get; set; } = new();
        public List<SLookupDto> Priorities { get; set; } = new();
    }

    public class BLAmendmentLineItemViewDto
    {
        public int Index { get; set; }

        public BLAmendmentRequestLineFormDto Line { get; set; } = new();
    }


}
