using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class EventTypeListItemDto
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string EventTypeCode { get; set; } = string.Empty;
        public string EventTypeName { get; set; } = string.Empty;
        public string EventTypeRemarks { get; set; } = string.Empty;
        public bool SystemDefined { get; set; }
        public bool IsSystemEvent { get; set; }
        public bool IsActive { get; set; }
    }


    public class EventTypeCategoryDto
    {
        public int Id { get; set; }
        public string EventTypeCategoryName { get; set; } = string.Empty;
        public string EventTypeCategoryLocalName { get; set; } = string.Empty;
    }

    public class CreateEventTypeDto
    {
        [Required]
        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(30)]
        public string EventTypeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EventTypeName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string EventTypeRemarks { get; set; } = string.Empty;

        public bool? SystemDefined { get; set; }
        public bool? IsSystemEvent { get; set; }
        public bool IsActive { get; set; } = true;
        public IEnumerable<SelectListItem>? Categories { get; set; } = new List<SelectListItem>();

    }

    public class UpdateEventTypeDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(30)]
        public string EventTypeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EventTypeName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string EventTypeRemarks { get; set; } = string.Empty;

        public bool SystemDefined { get; set; }
        public bool IsSystemEvent { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; } = new List<SelectListItem>();
    }


    public class EventTypeSearchDto
    {
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class EventTypeFormViewModel
    {
        public CreateEventTypeDto? CreateDto { get; set; } = new();
        public UpdateEventTypeDto? UpdateDto { get; set; } = new();

        public IEnumerable<SelectListItem>? Categories { get; set; } = new List<SelectListItem>();
    }

    public class EventTypeManagementViewModel
    {
        public IEnumerable<EventTypeCategoryDto> Categories { get; set; } = new List<EventTypeCategoryDto>();
        public int? SelectedCategoryId { get; set; }
    }
}
