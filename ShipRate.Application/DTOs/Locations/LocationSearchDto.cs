using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class LocationSearchDto
    {
        [Display(Name = "کشور")]
        public int? CountryId { get; set; }

        [Display(Name = "نوع مکان")]
        public EnumLocationType? LocationType { get; set; }

        [Display(Name = "جستجو (نام/کد)")]
        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
