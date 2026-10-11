using ShipRate.Common.Notifications.Models;

namespace ShipRate.Common.Notifications.Repository
{
    public interface INotificationRepository
    {

        public Task<List<Notification>> GetNewNotifications(int batchSize);

        public Task UpdateStatus(Guid notificationId, NotificationStatus status);
        public Task AddNotificationAsync(Notification notification);
    }
}
