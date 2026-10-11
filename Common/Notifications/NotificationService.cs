using ShipRate.Common.Notifications.Models;
using ShipRate.Common.Notifications.Repository;
using ShipRate.Common.Notifications.Senders;

namespace ShipRate.Common.Notifications
{
    public class NotificationService
    {
        private readonly NotificationConfig config;
        private readonly INotificationRepository notificationRepository;
        public NotificationService(NotificationConfig config, INotificationRepository repository)
        {
            this.notificationRepository = repository;
            this.config = config;
        }

        public async Task AddNotificationAsync(NotificationType type, string subject, string body, List<string> recipients, List<NotificationAttachment> attachments = null)
        {
            var id = Guid.CreateVersion7();
            var notification = new Notification
            {
                Id = id,
                Type = type,
                Subject = subject,
                Body = body,
                DateQueued = DateTime.Now,
                Status = NotificationStatus.NotSent,
                RetryCount = 0
            };
            var notifRecipients = new List<NotificationRecipient>();
            foreach (var item in recipients)
            {
                notifRecipients.Add(new NotificationRecipient
                {
                    Id = Guid.CreateVersion7(),
                    NotificationId = id,
                    Recipient = item,
                    NotificationStatus = NotificationStatus.NotSent,
                });
            }
            List<NotificationAttachment> notifAttachments = new List<NotificationAttachment>();
            if (attachments != null && type == NotificationType.Email)
            {
                foreach (var attachment in attachments)
                {
                    attachment.Id = id;
                    attachment.NotificationId = id;
                    notifAttachments.Add(attachment);
                }
            }
            notification.Attachments = notifAttachments;
            notification.NotificationRecipients = notifRecipients;
            await notificationRepository.AddNotificationAsync(notification);
        }
        public async Task SendNotification(Notification notification)
        {
            var sender = GetSender(notification.Type);
            var result = await sender.SendAsync(notification);
            if (result.Succeded)
            {
                await notificationRepository.UpdateStatus(notification.Id, NotificationStatus.Sent);
            }
            else
            {
                // set notif status to failed .
                await notificationRepository.UpdateStatus(notification.Id, NotificationStatus.Failed);
                Console.WriteLine($"Error in sending Notification - {result.Message}");
            }
        }

        private INotificationSender GetSender(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Email:
                    return new EmailSender(config.Email);
                case NotificationType.SMS:
                    return new SMSSender(config.SMS);
                case NotificationType.Application:
                    return new AppNotifSender();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
