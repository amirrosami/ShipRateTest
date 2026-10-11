using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class LocationStep1Dto
    {
        [Required(ErrorMessage = "انتخاب کشور الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفاً کشور را انتخاب کنید")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "نوع مکان الزامی است")]
        public EnumLocationType LocationType { get; set; }

        [Required(ErrorMessage = "کد مکان الزامی است")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "کد باید بین 2 تا 10 کاراکتر باشد")]
        [Display(Name = "کد مکان")]
        public string LocationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام مکان الزامی است")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "نام باید بین 2 تا 150 کاراکتر باشد")]
        [Display(Name = "نام مکان")]
        public string LocationName { get; set; } = string.Empty;
    }
}
