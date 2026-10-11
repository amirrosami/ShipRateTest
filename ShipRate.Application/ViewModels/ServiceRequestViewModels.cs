using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShipRate.Application.ViewModels
{
    // ─── Enums ────────────────────────────────────────────────────────────────────

    public enum ServiceRequestType
    {
        BLAmendment,
        Consolidation,
        Split
    }

    public enum ServiceRequestStatus
    {
        Pending,
        InReview,
        Approved,
        Rejected
    }

    // ─── List ─────────────────────────────────────────────────────────────────────

    public class ServiceRequestListItem
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = default!;
        public ServiceRequestType Type { get; set; }
        public string BLNumber { get; set; } = default!;
        public string JobRef { get; set; } = default!;
        public ServiceRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
    }

    // ─── Detail ───────────────────────────────────────────────────────────────────

    public class ServiceRequestDetail : ServiceRequestListItem
    {
        public string? Description { get; set; }
        public string? RejectionReason { get; set; }

        // BL Amendment
        public List<string>? AmendmentFields { get; set; }

        // Consolidation
        public string? TargetBLNumber { get; set; }

        // Split
        public int? SplitCount { get; set; }

        public List<ServiceRequestChargeItem> Charges { get; set; } = new();
        public List<ServiceRequestApprovalItem> Approvals { get; set; } = new();
    }

    public class ServiceRequestChargeItem
    {
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class ServiceRequestApprovalItem
    {
        public string Step { get; set; } = default!;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Status { get; set; } = default!;
    }

    // ─── Create Forms ─────────────────────────────────────────────────────────────

    public class CreateBLAmendmentViewModel
    {
        [Required] public string BLNumber { get; set; } = default!;
        [Required] public string JobRef { get; set; } = default;
        public List<SelectOption> AvailableBLs { get; set; } = new();
        public List<SelectOption> AvailableAmendmentFields { get; set; } = new();
        [Required] public List<string> SelectedAmendmentFields { get; set; } = new();

    }

    public class CreateBLAmendmentVM
    {
        [Required] public string BLNumber { get; set; } = default!;
        [Required] public string JobRef { get; set; } = default!;
        [Required] public string Description { get; set; } = default!;
        [Required] public List<string> SelectedAmendmentFields { get; set; } = new();

        public List<string> SourceBLNumbers { get; set; } = new();
        [Required] public string TargetBLNumber { get; set; } = default!;
        public List<SelectOption> AvailableAmendmentFields { get; set; } = new();
        public List<SelectOption> AvailableBLs { get; set; } = new();
    }
    public class CreateSplitViewModel
        {
            [Required] public string BLNumber { get; set; } = default!;
            [Required] public string JobRef { get; set; } = default!;
            [Required, Range(2, 10)] public int SplitCount { get; set; } = 2;
            public string? Description { get; set; }

            public List<SelectOption> AvailableBLs { get; set; } = new();
        }

    // ─── Shared ───────────────────────────────────────────────────────────────────


        public class CreateConsolidationViewModel
        {
        public Guid ServiceRequestId { get; set; }

        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public List<SelectOption> AvailableBLs { get; set; } = new();

    }

    public class SelectOption
        {
            public string Value { get; set; } = default!;
            public string Label { get; set; } = default!;
        }

    }
