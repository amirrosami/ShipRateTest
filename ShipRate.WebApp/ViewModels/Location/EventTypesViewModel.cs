using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.WebApp.ViewModels.Location
{
    public class EventTypesViewModel
    {
        public int Id { get; set; }
        public int? ObjectTypeId { get; set; }
        public int? CategoryId { get; set; }
        public string EventTypeCode { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string EventTypeRemarks { get; set; } = string.Empty;
        public bool SystemDefined { get; set; } = false;
        public bool IsSystemEvent { get; set; }
        public string EventTypeCategoryName { get; set; }

        public List<SelectListItem> EventCategories{ get; set; }
        public List<SelectListItem> SystemObjectTypes { get; set; }
    }
}