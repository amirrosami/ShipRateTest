using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Web.ViewModels
{
    public class SecurityEventIndexViewModel
    {
        // Filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? UserId { get; set; }
        public string? Email { get; set; }
        public string? EventType { get; set; }
        public bool? IsSuccessful { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        // Data
        public List<SecurityEventViewModel> Events { get; set; } = new();

        // Dropdowns
        public SelectList? Users { get; set; }

        // Statistics
        public int TotalFailedLogins { get; set; }
        public int TotalLockouts { get; set; }
        public int TotalPasswordChanges { get; set; }
    }
}
