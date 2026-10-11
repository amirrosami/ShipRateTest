using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class LocationBaseDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "انتخاب کشور الزامی است")]
        [Display(Name = "کشور")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "نوع مکان الزامی است")]
        [Display(Name = "نوع مکان")]
        public EnumLocationType LocationType { get; set; }

        [Required(ErrorMessage = "کد مکان الزامی است")]
        [StringLength(20, ErrorMessage = "کد نباید بیشتر از 20 کاراکتر باشد")]
        [Display(Name = "کد مکان")]
        public string LocationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام مکان الزامی است")]
        [StringLength(200, ErrorMessage = "نام نباید بیشتر از 200 کاراکتر باشد")]
        [Display(Name = "نام مکان")]
        public string LocationName { get; set; } = string.Empty;

        public string? CountryName { get; set; }
    }
}
