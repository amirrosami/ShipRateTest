namespace ShipRate.Application.Services.SMS
{
    public class SmsGatewayOptions
    {
        public const string SectionName = "SmsGateway";
        public bool IsEnabled { get; set; }

        public string BaseUrl { get; set; } = "https://www.payamsms.com";
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DefaultSender { get; set; } = string.Empty;
    }
}
