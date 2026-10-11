namespace ShipRate.Web.ViewModels
{
    public class AuditLogFilterViewModel
    {
        public string? UserName { get; set; }
        public Guid? UserId { get; set; }
        public string? ActionName { get; set; }
        public string? EntityName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // برای نمایش نتایج
        public List<AuditLogViewModel> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
