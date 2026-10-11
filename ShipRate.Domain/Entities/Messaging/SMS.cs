using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Messaging
{

    public enum SmsDeliveryStatus
    {
        Queued = 0,             // در صف ارسال دیتابیس
        Sending = 1,            // ارسال شده به سمت اپراتور
        SentSuccessfully = 2,    // تایید موفقیت ارسال
        Delivered = 3,          // تحویل نهایی به گوشی مخاطب
        Failed = 4,             // ناموفق / خطا
        PendingInQueue = 5      // در صف پردازش اپراتور
    }
    public class SmsMessage : BusinessEntity
    {
        public string Sender { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Tag { get; set; } // مثال: NOA, DO, OTP, Invoice
        public DateTime CreatedAtUtc { get; set; } = DateTime.Now;
        public DateTime? ScheduledForUtc { get; set; }

        public ICollection<SmsRecipient> Recipients { get; set; } = new List<SmsRecipient>();
    }

    public class SmsRecipient : BusinessEntity
    {
        public Guid SmsMessageId { get; set; }
        public SmsMessage? SmsMessage { get; set; }

        public string Mobile { get; set; } = string.Empty;
        public string CustomerTrackingId { get; set; } = Guid.NewGuid().ToString("N");
        public string? GatewayServerId { get; set; }

        public SmsDeliveryStatus Status { get; set; } = SmsDeliveryStatus.Queued;
        public string? ErrorCode { get; set; }
        public string? ErrorDescription { get; set; }
        public int RetryCount { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public DateTime? LastStatusCheckUtc { get; set; }
    }

    public class SmsIncomingMessage
    {
        public long Id { get; set; } // شناسه یکتا از سمت پنل پیامک
        public string LineNumber { get; set; } = string.Empty;
        public string FromMobile { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime ReceivedAtUtc { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class SmsMoCursor
    {
        public string LineNumber { get; set; } = string.Empty; // کلید اصلی
        public long LastProcessedId { get; set; }
    }

}
