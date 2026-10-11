using ShipRate.Common.Notifications.Models;

namespace ShipRate.Common.Notifications.Senders
{
    public class AppNotifSender : INotificationSender
    {
        public Task<SendNotificationResult> SendAsync(Notification notification)
        {
            throw new NotImplementedException();
        }
    }


}
