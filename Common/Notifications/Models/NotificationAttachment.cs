namespace ShipRate.Common.Notifications.Models
{
    public class NotificationAttachment
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string FileStorageReference { get; set; } = null!;
    }
}
