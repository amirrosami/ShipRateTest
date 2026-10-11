using ShipRate.Common.Notifications.Models;

namespace ShipRate.Common.Notifications
{
    public interface INotificationSender
    {
        public Task<SendNotificationResult> SendAsync(Notification notification);

    }
}
