namespace ShipRate.Web.ViewModels
{
    public class AuditLogViewModel
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? UserEmail { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // برای نمایش بهتر
        public string CreatedAtPersian =>
            new System.Globalization.PersianCalendar()
                .GetYear(CreatedAt) + "/" +
            new System.Globalization.PersianCalendar()
                .GetMonth(CreatedAt) + "/" +
            new System.Globalization.PersianCalendar()
                .GetDayOfMonth(CreatedAt);
    }
}
