namespace ShipRate.Web.ViewModels
{
    public class SecurityEventFilterViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? EventType { get; set; }
        public bool? IsSuccessful { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
