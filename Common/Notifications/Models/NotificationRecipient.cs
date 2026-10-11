namespace ShipRate.Common.Notifications.Models
{
    public class NotificationRecipient
    {
        public Guid Id { get; set; }
        public Guid NotificationId { get; set; }
        public string Recipient { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public Notification Notification { get; set; }
    }
}
