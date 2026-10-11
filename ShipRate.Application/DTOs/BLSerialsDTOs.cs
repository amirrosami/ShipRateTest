using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Application.DTOs
{
    // DTO برای جستجو
    public class BLSerialSeriesSearchDto
    {
        public Guid? CarrierBLTypeId { get; set; }
        public long? SerialNumber { get; set; } // جستجو بر اساس سریال موجود در BLSerials
    }

    // برای نمایش در گرید سری‌ها
    public class BLSerialSeriesListItemDto
    {
        public Guid Id { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public string Prefix { get; set; }
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string CarrierBLTypeName { get; set; }
        public bool IsActive { get; set; }
    }

    // برای فرم Create/Edit سری
    public class BLSerialSeriesEditDto
    {
        public Guid? Id { get; set; }
        public Guid CarrierBLTypeId { get; set; }
        public string SeriesCode { get; set; }
        public string SeriesName { get; set; }
        public string Prefix { get; set; }
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
    }

    public class BLSerialBatchAssignListItemDto
    {
        public Guid Id { get; set; }
        public Guid BLSerialSeriesId { get; set; }
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public DateTime AssignDate { get; set; }
        public string AgentName { get; set; }   // از Persons
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
    }

    public class BLSerialBatchAssignEditDto
    {
        public Guid? Id { get; set; }
        public Guid BLSerialSeriesId { get; set; }
        public Guid AgentId { get; set; }
        public long FromNumber { get; set; }
        public long ToNumber { get; set; }
        public DateTime AssignDate { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
    }
    public class BLSerialListItemDto
    {
        public Guid Id { get; set; }
        public Guid BLSerialSeriesId { get; set; }
        public long SerialNumber { get; set; }
        public string AgentName { get; set; }
        public string BLNo { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? AttachedAt { get; set; }
        public bool IsActive { get; set; }
    }
    public class BLSerialSeriesIndexViewModel
    {
        public BLSerialSeriesSearchDto Search { get; set; } = new();
        public IEnumerable<SelectListItem> CarrierBLTypes { get; set; } = Enumerable.Empty<SelectListItem>();
    }

}
