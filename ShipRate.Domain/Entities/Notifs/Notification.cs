using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.Notifs
{
    public class Notification : BusinessEntity
    {
        public int SourceTypeId { get; set; }
        public int NotificationTypeId { get; set; }
        public int SeverityId { get; set; }
        public int PriorityId { get; set; }

        public string Title { get; set; } = default!;
        public string? Message { get; set; }

        public string? ReferenceNo { get; set; }

        public int? EntityTypeId { get; set; }
        public Guid? EntityId { get; set; }

        public Guid? ActivityLogId { get; set; }

        public string? TargetUrl { get; set; }
        public string? IconCssClass { get; set; }

        public DateTime PublishAt { get; set; }
        public DateTime? ExpireAt { get; set; }

        public bool IsBroadcast { get; set; }
        public bool RequiresAction { get; set; }


        public bool? IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public Guid? ArchivedByUserId { get; set; }

        //Relations
        public NotificationSourceType SourceType { get; set; } = default!;
        public EventType NotificationType { get; set; } = default!;
        public NotificationSeverity Severity { get; set; } = default!;
        public FollowUpPriority Priority { get; set; } = default!;
        public ActivityLog? ActivityLog { get; set; }
        public EntityType? EntityType { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        public ApplicationUser? ArchivedByUser { get; set; }

        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        public ICollection<NotificationAudience> Audiences { get; set; } = new List<NotificationAudience>();
        public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
    }

    public class UserNotification : BusinessEntity
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public bool IsSeen { get; set; }
        public DateTime? SeenAt { get; set; }

        public bool IsHidden { get; set; }
        public DateTime? HiddenAt { get; set; }

        public bool IsPinned { get; set; }

        //Relations
        public Notification Notification { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
    }

    public class NotificationAudience : BusinessEntity
    {
        public Guid NotificationId { get; set; }
        public int AudienceTypeId { get; set; }

        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? CompanyId { get; set; }

        //Relations
        public Notification Notification { get; set; } = default!;
        public NotificationAudienceType AudienceType { get; set; } = default!;
    }
    public class NotificationDelivery : BusinessEntity
    {
        public Guid NotificationId { get; set; }
        public Guid? UserNotificationId { get; set; }
        public Guid? UserId { get; set; }

        public int ChannelId { get; set; }
        public int DeliveryStatusId { get; set; }

        public string? Destination { get; set; }

        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? FailedAt { get; set; }

        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }

        //Relations
        public Notification Notification { get; set; } = default!;
        public UserNotification? UserNotification { get; set; }
        public ApplicationUser? User { get; set; }
        public NotificationChannel Channel { get; set; } = default!;
        public NotificationDeliveryStatus DeliveryStatus { get; set; } = default!;
    }

    public class NotificationSourceType : SystemEntity
    {
        public string SourceTypeName { get; set; } = default!;
        public string? SourceTypeLocalName { get; set; } = default!;
    }
    public class NotificationSeverity : SystemEntity
    {
        public string SeverityName { get; set; } = default!;
        public string? SeverityLocalName { get; set; } = default!;
        public string? CssClass { get; set; }
        public string? IconCssClass { get; set; }
    }

    public class NotificationAudienceType : SystemEntity
    {
        public string AudienceTypeName { get; set; } = default!;
        public string? AudienceTypeLocalName { get; set; } = default!;
    }
    public class NotificationChannel : SystemEntity
    {
        public string ChannelName { get; set; } = default!;
        public string? ChannelLocalName { get; set; } = default!;
    }
    public class NotificationDeliveryStatus : SystemEntity
    {
        public string DeliveryStatusName { get; set; } = default!;
        public string? DeliveryStatusLocalName { get; set; } = default!;
    }

    public sealed class NotificationRule : BusinessEntity
    {
        public string ActivityType { get; set; } = "";
        public int EntityTypeId { get; set; }
        public bool ShouldNotify { get; set; } = true;

        public int RecipientModeId { get; set; }

        public string TitleTemplate { get; set; } = "";
        public string MessageTemplate { get; set; } = "";
        public string? TargetUrlTemplate { get; set; }

        public EntityType EntityType { get; set; }
        public List<NotificationRuleUser> Users { get; set; } = new();
        public List<NotificationRuleUserType> UserTypes { get; set; } = new();
        public NotificationRecipientMode RecipientMode { get; set; }

    }

    public sealed class NotificationRuleUser : BusinessEntity
    {
        public Guid NotificationRuleId { get; set; }
        public Guid UserId { get; set; }

        //Relations
        public ApplicationUser User { get; set; } = null!;
        public NotificationRule NotificationRule { get; set; } = null!;
    }
    public sealed class NotificationRuleUserType : BusinessEntity
    {
        public Guid NotificationRuleId { get; set; }
        public int UserTypeId { get; set; }


        //Relations
        public NotificationRule NotificationRule { get; set; } = null!;
        public UserType UserType { get; set; } = null!;
    }
    public enum EnumNotificationRecipientMode
    {
        None = -1,

        // فقط کاربر انجام‌دهنده عملیات
        Actor = 1,

        // همه ادمین‌ها
        Admins = 2,

        // کاربران مشخص‌شده در Rule
        SpecificUsers = 3,

        // همه کاربران یک یا چند UserType
        UserTypes = 4,

        // کاربران مرتبط با Entity، مثلا صاحب Shipment یا Job
        EntityRelatedUsers = 5,

        // ترکیبی از چند حالت
        Mixed = 6
    }


    public class NotificationRecipientMode : SystemEntity
    {
        public string RecipientModeName { get; set; } = string.Empty;
        public string RecipientModeLocalName { get; set; } = string.Empty;
    }
}
