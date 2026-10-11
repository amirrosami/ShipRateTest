using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Web.ViewModels
{
    public class AuditLogIndexViewModel
    {
        // Filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ActionName { get; set; }
        public string? EntityName { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }

        // Data
        public List<AuditLogViewModel> Logs { get; set; } = new();

        // برای Dropdown کاربران
        public SelectList? Users { get; set; }
    }
}