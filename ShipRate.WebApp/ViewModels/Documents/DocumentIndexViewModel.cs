using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;

namespace ShipRate.WebApp.ViewModels.Documents
{
    public class DocumentIndexViewModel
    {
        public DocumentSearchDto Search { get; set; } = new();
        public PagedResultDto<DocumentListItemDto> Result { get; set; } = new();

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> DocumentTypes { get; set; } = new();
        public List<SelectListItem> ObjectTypes { get; set; } = new();
        public List<SelectListItem> IsActiveList { get; set; } = new();
        public List<SelectListItem> PageSizeList { get; set; } = new();
    }
}
