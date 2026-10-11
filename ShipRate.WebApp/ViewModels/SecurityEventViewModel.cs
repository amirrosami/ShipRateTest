namespace ShipRate.Web.ViewModels
{
    public class SecurityEventViewModel
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? AttemptedEmail { get; set; } // برای Failed Login
        public string EventType { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccessful { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
