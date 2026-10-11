namespace ShipRate.Common.Notifications.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }

        public List<string> Recipients { get; set; } = new();

        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateQueued { get; set; }
        public NotificationStatus Status { get; set; } // "NotSent", "Sent", "Failed"
        public int RetryCount { get; set; }
        public string? LastError { get; set; }
        public ICollection<NotificationRecipient> NotificationRecipients { get; set; } = new List<NotificationRecipient>();
        public ICollection<NotificationAttachment> Attachments { get; set; } = new List<NotificationAttachment>();

    }
}
