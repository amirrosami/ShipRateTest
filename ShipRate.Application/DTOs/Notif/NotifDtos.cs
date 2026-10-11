using ShipRate.Domain.Entities.Notifs;

namespace ShipRate.Application.DTOs.Notif
{
    public class SelectListItemDto
    {
        public string Value { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public bool Selected { get; set; }
    }

    public class CreateNotificationDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Message { get; set; }

        public int NotificationTypeId { get; set; }

        public int SeverityId { get; set; }

        public int PriorityId { get; set; }

        public string? ReferenceNo { get; set; }

        public string? TargetUrl { get; set; }

        public string? IconCssClass { get; set; }

        public DateTime PublishAt { get; set; } = DateTime.Now;

        public DateTime? ExpireAt { get; set; }

        public bool RequiresAction { get; set; }

        public int? SelectedUserTypeId { get; set; }

        public List<Guid> SelectedUserIds { get; set; } = new();

        public List<SelectListItemDto> NotificationTypes { get; set; } = new();

        public List<SelectListItemDto> Severities { get; set; } = new();

        public List<SelectListItemDto> Priorities { get; set; } = new();

        public List<SelectListItemDto> UserTypes { get; set; } = new();

        public List<SelectListItemDto> Users { get; set; } = new();
    }

    public class CreateManualNotificationRequest
    {
        public string Title { get; set; } = string.Empty;

        public string? Message { get; set; }

        public int NotificationTypeId { get; set; }

        public int SeverityId { get; set; }

        public int PriorityId { get; set; }

        public string? ReferenceNo { get; set; }

        public string? TargetUrl { get; set; }

        public string? IconCssClass { get; set; }

        public DateTime PublishAt { get; set; }

        public DateTime? ExpireAt { get; set; }

        public bool RequiresAction { get; set; }

        public int SelectedUserTypeId { get; set; }

        public List<Guid> SelectedUserIds { get; set; } = new();
    }
    public class NavbarNotificationDto
    {
        public Guid UserNotificationId { get; set; }

        public Guid NotificationId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Message { get; set; }

        public string? TargetUrl { get; set; }

        public string? IconCssClass { get; set; }

        public int SeverityId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class NavbarNotificationsViewModel
    {
        public int UnreadCount { get; set; }

        public List<NavbarNotificationDto> Items { get; set; } = new();
    }

    public class NotificationListItemDto
    {
        public Guid UserNotificationId { get; set; }

        public Guid NotificationId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Message { get; set; }

        public string? ReferenceNo { get; set; }

        public string? TargetUrl { get; set; }

        public string? IconCssClass { get; set; }

        public int SeverityId { get; set; }

        public bool IsRead { get; set; }

        public bool IsSeen { get; set; }

        public bool IsPinned { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }
    }
    public class NotificationListFilterDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public bool? IsRead { get; set; }

        public string? SearchTerm { get; set; }
    }


    public class ServiceResultDto
    {
        public bool Succeeded { get; set; }

        public string? Message { get; set; }

        public static ServiceResultDto Success(string? message = null)
        {
            return new ServiceResultDto
            {
                Succeeded = true,
                Message = message
            };
        }

        public static ServiceResultDto Fail(string message)
        {
            return new ServiceResultDto
            {
                Succeeded = false,
                Message = message
            };
        }
    }

    public class ServiceResultDto<T>
    {
        public bool Succeeded { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public static ServiceResultDto<T> Success(T data, string? message = null)
        {
            return new ServiceResultDto<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static ServiceResultDto<T> Fail(string message)
        {
            return new ServiceResultDto<T>
            {
                Succeeded = false,
                Message = message
            };
        }
    }



    public sealed class NotificationAdminFilterDto
    {
        public string? Search { get; set; }
        public int? UserTypeId { get; set; }
        public int? SourceTypeId { get; set; }
        public bool? IsArchived { get; set; }
        public bool? HasUnread { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public sealed class NotificationAdminListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string MessagePreview { get; set; } = "";
        public string SourceTypeTitle { get; set; } = "";
        public string CreatedByName { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public int TotalRecipients { get; set; }
        public int ReadCount { get; set; }
        public int UnreadCount => TotalRecipients - ReadCount;

        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }

    public sealed class NotificationAdminListViewModel
    {
        public NotificationAdminFilterDto Filter { get; set; } = new();
        public List<NotificationAdminListItemDto> Items { get; set; } = new();

        public int TotalCount { get; set; }
        public int Page => Filter.Page;
        public int PageSize => Filter.PageSize;
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
    public sealed class NotificationDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? TargetUrl { get; set; }
        public string? IconCssClass { get; set; }

        public string SourceTypeTitle { get; set; } = "";
        public string CreatedByName { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public int TotalRecipients { get; set; }
        public int ReadCount { get; set; }
        public int UnreadCount => TotalRecipients - ReadCount;

        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public string? ArchivedByName { get; set; }

        public List<NotificationRecipientDto> Recipients { get; set; } = new();
    }


    public sealed class NotificationRecipientDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string UserTypeTitle { get; set; } = "";

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class NotificationRuleDto
    {
        public string ActivityType { get; set; } = "";
        public string EntityType { get; set; } = "";

        public bool ShouldNotify { get; set; }
        public bool ExcludeActor { get; set; } = true;

        public NotificationRecipientMode RecipientMode { get; set; }
        public int RecipientModeId { get; set; }

        public List<Guid> UserIds { get; set; } = new();
        public List<int> UserTypeIds { get; set; } = new();

        public string TitleTemplate { get; set; } = "";
        public string MessageTemplate { get; set; } = "";
        public string? TargetUrlTemplate { get; set; }
    }

}
