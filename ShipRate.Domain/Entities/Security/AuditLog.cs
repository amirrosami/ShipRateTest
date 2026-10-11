namespace ShipRate.Domain.Entities.Security
{
    public class AuditLog
    {
        public int Id { get; set; }

        // User info
        public Guid UserId { get; set; }
        public string? UserName { get; set; }

        // Action info
        public string Action { get; set; } = string.Empty;      // Login, Logout, Create, Update, Delete
        public string EntityName { get; set; } = string.Empty;  // Country, City, User
        public string? EntityId { get; set; }

        // Data snapshot
        public string? OldValues { get; set; }   // JSON
        public string? NewValues { get; set; }   // JSON

        // Request info
        public string IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; }

        // Meta
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public ApplicationUser? User { get; set; }
    }
}
