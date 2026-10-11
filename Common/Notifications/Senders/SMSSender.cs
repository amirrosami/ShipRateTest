using IPE.SmsIrClient;
using ShipRate.Common.Notifications.Models;

namespace ShipRate.Common.Notifications.Senders
{
    public class SMSSender : INotificationSender
    {
        private readonly SMSConfig config;
        public SMSSender(SMSConfig config)
        {
            this.config = config;
        }

        public async Task<SendNotificationResult> SendAsync(Notification notification)
        {
            if (notification.Recipients.Count == 0)
            {
                return new SendNotificationResult { Message = "Recipients are assigned for this request", Succeded = false };
            }
            string message = $"{notification.Subject} \n {notification.Body}";
            SmsIr smsIr = new SmsIr(config.ApiKey);
            var bulkSendResult = await smsIr.BulkSendAsync(config.LineNumber, message, notification.Recipients.ToArray());
            if (bulkSendResult.Status == 1)
            {
                return new SendNotificationResult { Succeded = true };
            }
            return new SendNotificationResult
            {
                Succeded = false,
                Message = bulkSendResult.Message
            };
        }
    }
}
