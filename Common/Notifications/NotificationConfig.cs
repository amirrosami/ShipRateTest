namespace ShipRate.Common.Notifications
{
    public class NotificationConfig
    {
        public EmailConfig Email { get; set; }
        public SMSConfig SMS { get; set; }
        public AppNotifConfig AppNotif { get; set; }
    }

    public class NotificationWorkerConfig
    {
        public int IntervalSeconds { get; set; }
        public int NotifBatchSize { get; set; }
    }
    public class AppNotifConfig { }
    public class SMSConfig
    {
        public long LineNumber { get; set; }
        public string Uri { get; set; }
        public string ApiKey { get; set; }
    }
    public class EmailConfig
    {
        public string SenderAddress { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
    }
}
