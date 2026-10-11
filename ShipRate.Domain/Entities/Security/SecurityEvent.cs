namespace ShipRate.Domain.Entities.Security
{
    public class SecurityEvent
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string? Email { get; set; } // برای Failed Login که UserId نداریم
        public string EventType { get; set; } = string.Empty; // FailedLogin, Lockout, PasswordChange, etc.
        public string? Reason { get; set; } // Invalid Password, Account Locked, etc.
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccessful { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ApplicationUser? User { get; set; }
    }
}
