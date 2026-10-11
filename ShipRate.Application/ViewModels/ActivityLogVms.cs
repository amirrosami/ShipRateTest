using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class CreateActivityLogRequest
    {
        // Main target entity
        [Required]
        [MaxLength(100)]
        public int EntityTypeId { get; set; }

        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public int ActivityCategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ActivityType { get; set; } = null!;

        // Optional event type references
        public int? EventTypeId { get; set; }

        // Display fields
        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = null!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? ReferenceNo { get; set; }

        public DateTime? ActivityDateTime { get; set; }

        // Generic status change fields
        [MaxLength(100)]
        public string? FromStatusEntityType { get; set; }

        public int? FromStatusId { get; set; }

        [MaxLength(100)]
        public string? ToStatusEntityType { get; set; }

        public int? ToStatusId { get; set; }

        [MaxLength(50)]
        public string? ToStatusCode { get; set; }

        [MaxLength(150)]
        public string? ToStatusName { get; set; }

        // History / audit fields
        [MaxLength(150)]
        public string? FieldName { get; set; }

        [MaxLength(2000)]
        public string? OldValue { get; set; }

        [MaxLength(2000)]
        public string? NewValue { get; set; }

        public string? ChangeSetJson { get; set; }


        // Location
        public Guid? LocationId { get; set; }

        // User / actor
        public Guid? PerformedByUserId { get; set; }

        public bool IsSystemGenerated { get; set; }
        public bool VisibleForCustomers { get; set; } = false;


        [MaxLength(100)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        // UI fields
        [MaxLength(50)]
        public string? Severity { get; set; }

        [MaxLength(100)]
        public string? IconCssClass { get; set; }

        // Support / integration
        [MaxLength(1000)]
        public string? InternalNote { get; set; }

        [MaxLength(100)]
        public string? SourceSystem { get; set; }

        [MaxLength(150)]
        public string? ExternalReferenceNo { get; set; }



    }

    public class ActivityLogListItemVm
    {
        public Guid Id { get; set; }

        // Main target entity
        public string EntityType { get; set; } = null!;

        public Guid EntityId { get; set; }

        // Classification
        public string ActivityCategory { get; set; } = null!;

        public string ActivityType { get; set; } = null!;

        // Event type display
        public Guid? EventTypeId { get; set; }

        public string? EventTypeName { get; set; }

        public Guid? EventTypeCategoryId { get; set; }

        public string? EventTypeCategoryName { get; set; }

        // Main display
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime ActivityDateTime { get; set; }
        public string ActivityDateLocal { get; set; }

        // Status display
        public string? FromStatusEntityType { get; set; }

        public Guid? FromStatusId { get; set; }

        public string? FromStatusCode { get; set; }

        public string? FromStatusName { get; set; }

        public string? ToStatusEntityType { get; set; }

        public Guid? ToStatusId { get; set; }

        public string? ToStatusCode { get; set; }

        public string? ToStatusName { get; set; }

        // History / audit display
        public string? FieldName { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? ChangeSetJson { get; set; }

        // Related entity ids
        public Guid? RateRequestId { get; set; }

        public Guid? BookingId { get; set; }

        public Guid? BlId { get; set; }

        public Guid? ContractId { get; set; }

        public Guid? ShipmentId { get; set; }

        public Guid? JobId { get; set; }

        // Common references
        public Guid? CompanyId { get; set; }

        public Guid? PersonId { get; set; }

        public Guid? CustomerId { get; set; }

        // Location
        public Guid? LocationId { get; set; }

        public string? LocationName { get; set; }

        // Actor
        public Guid? PerformedByUserId { get; set; }

        public string? PerformedByUserName { get; set; }

        public bool IsSystemGenerated { get; set; }

        // UI
        public string? Severity { get; set; }

        public string? IconCssClass { get; set; }

        // Optional support fields
        public string? InternalNote { get; set; }

        public string? SourceSystem { get; set; }

        public string? ExternalReferenceNo { get; set; }

        public bool VisibleForCustomers { get; set; }

    }

    public class ActivityTimelineItemVm
    {
        public Guid Id { get; set; }

        public string ActivityCategory { get; set; } = null!;

        public string ActivityType { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime ActivityDateTime { get; set; }

        public string? PerformedByUserName { get; set; }

        public string? FromStatusName { get; set; }

        public string? ToStatusName { get; set; }

        public string? FieldName { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? Severity { get; set; }

        public string? IconCssClass { get; set; }



    }


    public class EntityTimelineItemVm
    {
        public string Title { get; set; } = "";
        public DateTime Date { get; set; }
        public string LocalDate { get; set; } = "";
        public string Description { get; set; } = "";
    }

    // جهت لود داده‌های تکی برای فرم ویرایش
    public class ActivityLogDetailsVm
    {
        public Guid Id { get; set; }
        public int EntityTypeId { get; set; }
        public Guid EntityId { get; set; }
        public int? EventTypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ExternalReferenceNo { get; set; }
        public string? InternalNote { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public bool VisibleForCustomers { get; set; }
    }


    // درخواست ویرایش
    public class UpdateActivityLogRequest
    {
        public Guid Id { get; set; }
        public int? EventTypeId { get; set; }
        public string? ExternalReferenceNo { get; set; }
        public string? InternalNote { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public bool VisibleForCustomers { get; set; }
    }

    // جهت پر کردن Dropdown لیست نوع رویدادها
    public class EventTypeLookupVm
    {
        public int Id { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
    }

    public class ActivityLogWrapperVm
    {
        public int EntityTypeId { get; set; }
        public Guid EntityId { get; set; }
        public string? ExternalReferenceNo { get; set; }
    }

    public class ActivityLogGridVm
    {
        public int EntityTypeId { get; set; }
        public Guid EntityId { get; set; }
        public List<ActivityLogListItemVm> Items { get; set; } = new();
    }

    public class ActivityLogUpsertFormVm
    {
        public Guid? Id { get; set; }
        public int EntityTypeId { get; set; }
        public Guid EntityId { get; set; }

        public int? EventTypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ExternalReferenceNo { get; set; }
        public string? InternalNote { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public bool VisibleForCustomers { get; set; }

        public List<SelectListItem> EventTypes { get; set; } = new();
    }

    public class RecentActivityViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ReferenceNo { get; set; }
        public DateTime ActivityDateTime { get; set; }

        // Info | Success | Warning | Danger | Primary
        public string Severity { get; set; } = "Primary";
        public string IconCssClass { get; set; } = "bi-activity";

        public bool IsFresh => (DateTime.Now - ActivityDateTime).TotalMinutes < 1;


        public string? PerformedByUserName { get; set; }
        public string? LocationName { get; set; }
        public string? SourceSystem { get; set; }

        // برای نمایش متن نسبی مثل "2 min ago"
        public string RelativeTime => GetRelativeTime(ActivityDateTime);

        private static string GetRelativeTime(DateTime dt)
        {
            var span = DateTime.Now - dt;
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} min ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hr ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays} day(s) ago";
            return dt.ToString("MMM dd, yyyy");
        }
    }




}
