using ShipRate.Common.Notifications.Models;
using System.Net;
using System.Net.Mail;

namespace ShipRate.Common.Notifications.Senders
{
    public class EmailSender : INotificationSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailConfig config;
        public EmailSender(EmailConfig config)
        {
            this.config = config;
            _smtpClient = new SmtpClient(config.Host, config.Port)
            {
                Credentials = new NetworkCredential(config.SenderAddress, config.Password),
                EnableSsl = config.EnableSSL
            };
        }
        public async Task<SendNotificationResult> SendAsync(Notification notification)
        {
            using var message = new MailMessage()
            {
                From = new MailAddress(config.SenderAddress),
                Subject = notification.Subject,
                Body = notification.Body,
                IsBodyHtml = false
            };

            // Add recipients
            foreach (var recipient in notification.NotificationRecipients)
                message.To.Add(recipient.Recipient);

            if (notification.Attachments != null)
            {
                foreach (var att in notification.Attachments)
                {
                    var stream = new MemoryStream();
                    var mailAttachment = new Attachment(stream, att.FileName, att.ContentType);
                    message.Attachments.Add(mailAttachment);
                }
            }
            try
            {
                await _smtpClient.SendMailAsync(message);
                return new SendNotificationResult
                {
                    Succeded = true,
                    Status = 200,
                    erorrCode = ""
                };
            }
            catch (Exception e)
            {
                return new SendNotificationResult
                {
                    Succeded = false,
                    erorrCode = "500",
                    Status = 500,
                };
            }
        }

    }


}
