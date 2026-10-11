using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.ServiceRequests
{
    #region Main Request

    public class ServiceRequest : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int InOutId { get; set; }
        public string RequestNo { get; set; } = null!;
        public int RequestTypeId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public string Subject { get; set; } = null!;
        public string? Description { get; set; }
        public Guid? PartyId { get; set; }
        public string? PartyDetailName { get; set; }
        public string? ExternalReferenceNo { get; set; }
        public Guid? ParentServiceRequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        public bool IsChargeable { get; set; }
        public decimal? EstimatedCostAmount { get; set; }
        public decimal? FinalCostAmount { get; set; }
        public int? CostCurrencyId { get; set; }

        public string? Remarks { get; set; }

        //Relations
        public ServiceRequestType RequestType { get; set; }
        public ServiceRequestStatus RequestStatus { get; set; }
        public ServiceRequestPriority RequestPriority { get; set; }

        public Company Company { get; set; }
        public InOut InOut { get; set; }
        public Person Party { get; set; }
        public Currency CostCurrency { get; set; }
        public virtual ServiceRequest? ParentServiceRequest { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser? UpdateUser { get; set; }


        public virtual ICollection<ServiceRequest> ChildServiceRequests { get; set; } = new List<ServiceRequest>();
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual ICollection<ServiceRequestCharge> Costs { get; set; } = new List<ServiceRequestCharge>();
        public virtual ICollection<ServiceRequestApproval> Approvals { get; set; } = new List<ServiceRequestApproval>();

        //public virtual BLAmendmentRequest? BLAmendmentRequest { get; set; }
        //public virtual ConsolidationRequest? ConsolidationRequest { get; set; }
        //public virtual SplitRequest? SplitRequest { get; set; }

    }

    #endregion

    #region Cost / Approval

    public class ServiceRequestCharge : BusinessEntity
    {
        public Guid ServiceRequestId { get; set; }
        public int ChargeTypeId { get; set; }
        public string? CostTitle { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }

        public bool IsPayable { get; set; }
        public bool IsReceivable { get; set; }
        public bool IsApproved { get; set; }

        public string? Remarks { get; set; }

        //Relations
        public virtual ServiceRequest ServiceRequest { get; set; } = null!;
        public ChargeType ChargeType { get; set; }
        public Currency Currency { get; set; }
    }

    public class ServiceRequestApproval : BusinessEntity
    {
        public Guid ServiceRequestId { get; set; }

        public int ApprovalTypeId { get; set; }
        public int ApprovalStatusId { get; set; }

        public Guid? ApproverUserId { get; set; }
        public string? ApproverName { get; set; }

        public DateTime? DecisionDate { get; set; }
        public string? Remarks { get; set; }


        //Relations
        public virtual ServiceRequest ServiceRequest { get; set; } = null!;
        public ServiceRequestApprovalType ApprovalType { get; set; }
        public ServiceRequestApprovalStatus ApprovalStatus { get; set; }
        public ApplicationUser ApproverUser { get; set; }

    }

    #endregion

    #region BL Amendment

    public class BLAmendmentRequest : BusinessEntity
    {
        public Guid ServiceRequestId { get; set; }
        public Guid BLId { get; set; }

        public string? Reason { get; set; }

        public bool RequiresCarrierApproval { get; set; }
        public bool RequiresCustomerApproval { get; set; }

        //Relations
        public virtual ServiceRequest ServiceRequest { get; set; } = null!;
        public virtual BL BL { get; set; } = null!;
        public virtual ICollection<BLAmendmentRequestLine> Lines { get; set; } = new List<BLAmendmentRequestLine>();
    }

    public class BLAmendmentRequestLine : BusinessEntity
    {
        public Guid BLAmendmentRequestId { get; set; }

        public int FieldTypeId { get; set; }

        public string? CurrentValue { get; set; }
        public string? RequestedValue { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public virtual BLAmendmentRequest BLAmendmentRequest { get; set; } = null!;
        public BLAmendmentFieldType FieldType { get; set; }
    }

    #endregion

    #region Consolidation

    public class ConsolidationRequest : BusinessEntity
    {
        public Guid ServiceRequestId { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }

        //Relations
        public virtual ServiceRequest ServiceRequest { get; set; } = null!;
        public virtual ICollection<ConsolidationRequestItem> Items { get; set; } = new List<ConsolidationRequestItem>();
    }

    public class ConsolidationRequestItem : BusinessEntity
    {
        public Guid ConsolidationRequestId { get; set; }

        public Guid? SourceJobId { get; set; }
        public Guid? SourceBookingId { get; set; }
        public Guid? SourceBLId { get; set; }
        public Guid? SourceBLContainerId { get; set; }

        public string? Remarks { get; set; }

        //Relations
        public virtual ConsolidationRequest ConsolidationRequest { get; set; } = null!;
        public Job SourceJob { get; set; }
        public Booking SourceBooking { get; set; }
        public BL SourceBL { get; set; }
        public BLContainer SourceBLContainer { get; set; }
    }

    #endregion


    #region Split

    public class SplitRequest : BusinessEntity
    {
        public Guid ServiceRequestId { get; set; }

        public Guid? SourceJobId { get; set; }
        public Guid? SourceBookingId { get; set; }
        public Guid? SourceBLId { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }

        public bool IsFinalized { get; set; }

        // Relations
        public virtual ServiceRequest ServiceRequest { get; set; } = null!;

        public virtual Job? SourceJob { get; set; }
        public virtual Booking? SourceBooking { get; set; }
        public virtual BL? SourceBL { get; set; }

        public virtual ICollection<SplitRequestTarget> Targets { get; set; } = new List<SplitRequestTarget>();
    }

    public class SplitRequestTarget : BusinessEntity
    {
        public Guid SplitRequestId { get; set; }

        public string TargetKey { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? TargetBLNo { get; set; }
        public string? Remarks { get; set; }
        public int SortOrder { get; set; }

        // Relations
        public virtual SplitRequest SplitRequest { get; set; } = null!;
        public virtual ICollection<SplitRequestAllocation> Allocations { get; set; } = new List<SplitRequestAllocation>();
    }

    public class SplitRequestAllocation : BusinessEntity
    {
        public Guid SplitRequestTargetId { get; set; }

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

        // Relations
        public virtual SplitRequestTarget SplitRequestTarget { get; set; } = null!;
    }


    public static class SplitSourceLineTypes
    {
        public const int ContainerAllocation = 1;
        public const int DirectCargo = 2;
    }


    #endregion


    #region Lookup Tables Instead of Enums

    public class ServiceRequestType : SystemEntity
    {
        public string Name { get; set; } = null!;
        public string? LocalName { get; set; } = null!;
    }

    public class ServiceRequestStatus : SystemEntity
    {
        public string Name { get; set; } = null!;
        public string? LocalName { get; set; } = null!;
    }

    public class ServiceRequestPriority : SystemEntity
    {
        public string Name { get; set; } = null!;
        public string? LocalName { get; set; } = null!;
    }

    public class ServiceRequestActionType : SystemEntity
    {
        public int Code { get; set; }
        public string? Name { get; set; } = null!;
    }
    public class ServiceRequestApprovalType : SystemEntity
    {
        public string Name { get; set; } = null!;
        public string? LocalName { get; set; } = null!;
    }

    public class ServiceRequestApprovalStatus : SystemEntity
    {
        public string Name { get; set; } = null!;
        public string? LocalName { get; set; } = null!;
    }

    public class BLAmendmentFieldType : SystemEntity
    {
        public string Name { get; set; } = null!;
        public string? LocalName { get; set; } = null!;
    }

    #endregion

}
















//    public static class ServiceRequestLookupSeed
//    {
//        public static readonly object[] ServiceRequestTypes =
//        {
//            new { Id = 1, Name = "BL Amendment" },
//            new { Id = 2, Name = "Consolidation" },
//            new { Id = 3, Name = "Split" },
//            new { Id = 4, Name = "Change Consignee" },
//            new { Id = 5, Name = "Change Notify Party" },
//            new { Id = 6, Name = "Change Delivery Order" },
//            new { Id = 7, Name = "Change Container Allocation" },
//            new { Id = 8, Name = "Document Correction" },
//            new { Id = 9, Name = "Cargo Information Correction" },
//            new { Id = 10, Name = "Freight Revision" },
//            new { Id = 11, Name = "Customs Related Request" },
//            new { Id = 12, Name = "Carrier / Agent Follow-up" },
//            new { Id = 13, Name = "Internal Operation Request" },
//            new { Id = 99, Name = "Other" }
//        };

//        public static readonly object[] ServiceRequestStatuses =
//        {
//            new { Id = 1, Name = "Draft" },
//            new { Id = 2, Name = "Registered" },
//            new { Id = 3, Name = "Under Review" },
//            new { Id = 4, Name = "Waiting For Customer" },
//            new { Id = 5, Name = "Waiting For Carrier" },
//            new { Id = 6, Name = "Waiting For Agent" },
//            new { Id = 7, Name = "Waiting For Internal Approval" },
//            new { Id = 8, Name = "Approved" },
//            new { Id = 9, Name = "Rejected" },
//            new { Id = 10, Name = "In Progress" },
//            new { Id = 11, Name = "Completed" },
//            new { Id = 12, Name = "Closed" },
//            new { Id = 13, Name = "Cancelled" }
//        };

//        public static readonly object[] ServiceRequestPriorities =
//        {
//            new { Id = 1, Name = "Low" },
//            new { Id = 2, Name = "Normal" },
//            new { Id = 3, Name = "High" },
//            new { Id = 4, Name = "Urgent" },
//            new { Id = 5, Name = "Critical" }
//        };

//        public static readonly object[] RequestPartyTypes =
//        {
//            new { Id = 1, Name = "Customer" },
//            new { Id = 2, Name = "Shipper" },
//            new { Id = 3, Name = "Consignee" },
//            new { Id = 4, Name = "Notify Party" },
//            new { Id = 5, Name = "Carrier" },
//            new { Id = 6, Name = "Airline" },
//            new { Id = 7, Name = "Shipping Line" },
//            new { Id = 8, Name = "Agent" },
//            new { Id = 9, Name = "Vendor" },
//            new { Id = 10, Name = "Customs Broker" },
//            new { Id = 11, Name = "Internal Department" },
//            new { Id = 12, Name = "Branch" },
//            new { Id = 99, Name = "Other" }
//        };

//        public static readonly object[] ServiceRequestActionTypes =
//        {
//            new { Id = 1, Name = "Created" },
//            new { Id = 2, Name = "Updated" },
//            new { Id = 3, Name = "Submitted" },
//            new { Id = 4, Name = "Assigned" },
//            new { Id = 5, Name = "Reassigned" },
//            new { Id = 6, Name = "Status Changed" },
//            new { Id = 7, Name = "Approved" },
//            new { Id = 8, Name = "Rejected" },
//            new { Id = 9, Name = "Requested More Info" },
//            new { Id = 10, Name = "Comment Added" },
//            new { Id = 11, Name = "Attachment Added" },
//            new { Id = 12, Name = "Cost Added" },
//            new { Id = 13, Name = "Completed" },
//            new { Id = 14, Name = "Closed" },
//            new { Id = 15, Name = "Cancelled" }
//        };

//        public static readonly object[] ServiceRequestAttachmentTypes =
//        {
//            new { Id = 1, Name = "Customer Request Letter" },
//            new { Id = 2, Name = "BL Copy" },
//            new { Id = 3, Name = "Invoice" },
//            new { Id = 4, Name = "Packing List" },
//            new { Id = 5, Name = "Approval Document" },
//            new { Id = 6, Name = "Carrier Response" },
//            new { Id = 7, Name = "Agent Response" },
//            new { Id = 8, Name = "Supporting Document" },
//            new { Id = 9, Name = "Internal Memo" },
//            new { Id = 99, Name = "Other" }
//        };

//        public static readonly object[] ServiceRequestCostTypes =
//        {
//            new { Id = 1, Name = "Amendment Fee" },
//            new { Id = 2, Name = "Carrier Fee" },
//            new { Id = 3, Name = "Agent Fee" },
//            new { Id = 4, Name = "Documentation Fee" },
//            new { Id = 5, Name = "Handling Fee" },
//            new { Id = 6, Name = "Penalty" },
//            new { Id = 7, Name = "Internal Cost" },
//            new { Id = 99, Name = "Other" }
//        };

//        public static readonly object[] ServiceRequestApprovalTypes =
//        {
//            new { Id = 1, Name = "Customer Approval" },
//            new { Id = 2, Name = "Carrier Approval" },
//            new { Id = 3, Name = "Agent Approval" },
//            new { Id = 4, Name = "Finance Approval" },
//            new { Id = 5, Name = "Operation Approval" },
//            new { Id = 6, Name = "Management Approval" },
//            new { Id = 99, Name = "Other" }
//        };

//        public static readonly object[] ServiceRequestApprovalStatuses =
//        {
//            new { Id = 1, Name = "Pending" },
//            new { Id = 2, Name = "Approved" },
//            new { Id = 3, Name = "Rejected" },
//            new { Id = 4, Name = "Cancelled" },
//            new { Id = 5, Name = "Not Required" }
//        };

//        public static readonly object[] BLAmendmentFieldTypes =
//        {
//            new { Id = 1, Name = "Shipper" },
//            new { Id = 2, Name = "Consignee" },
//            new { Id = 3, Name = "Notify Party" },
//            new { Id = 4, Name = "Description Of Goods" },
//            new { Id = 5, Name = "Gross Weight" },
//            new { Id = 6, Name = "Net Weight" },
//            new { Id = 7, Name = "Measurement" },
//            new { Id = 8, Name = "Marks And Numbers" },
//            new { Id = 9, Name = "Package Count" },
//            new { Id = 10, Name = "Package Type" },
//            new { Id = 11, Name = "Container No" },
//            new { Id = 12, Name = "Seal No" },
//            new { Id = 13, Name = "Voyage No" },
//            new { Id = 14, Name = "Vessel Name" },
//            new { Id = 15, Name = "Flight No" },
//            new { Id = 16, Name = "Airline Name" },
//            new { Id = 17, Name = "Truck No" },
//            new { Id = 18, Name = "Driver Name" },
//            new { Id = 19, Name = "POL" },
//            new { Id = 20, Name = "POD" },
//            new { Id = 21, Name = "Final Destination" },
//            new { Id = 22, Name = "Place Of Receipt" },
//            new { Id = 23, Name = "Place Of Delivery" },
//            new { Id = 24, Name = "Freight Term" },
//            new { Id = 25, Name = "Freight Prepaid / Collect" },
//            new { Id = 26, Name = "Issue Date" },
//            new { Id = 27, Name = "Declared Value" },
//            new { Id = 28, Name = "Handling Information" },
//            new { Id = 29, Name = "Document No" },
//            new { Id = 30, Name = "Reference No" },
//            new { Id = 99, Name = "Other" }
//        };
//    }
//}
