using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class CityDto : LocationBaseDto
    {
        [Display(Name = "استان")]
        public Guid? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }

        [Required(ErrorMessage = "کد ISO شهر الزامی است")]
        [StringLength(10, ErrorMessage = "کد ISO نباید بیشتر از 10 کاراکتر باشد")]
        [Display(Name = "کد ISO")]
        public string CityIsoCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوع شهر الزامی است")]
        [Display(Name = "نوع شهر")]
        public EnumCityType CityType { get; set; }

        [Display(Name = "شهر مادر (برای بنادر)")]
        public Guid? PortCityId { get; set; }
        public string? PortCityName { get; set; }

        [Range(-90, 90, ErrorMessage = "عرض جغرافیایی باید بین -90 تا 90 باشد")]
        [Display(Name = "عرض جغرافیایی")]
        public decimal CityLatitude { get; set; }

        [Range(-180, 180, ErrorMessage = "طول جغرافیایی باید بین -180 تا 180 باشد")]
        [Display(Name = "طول جغرافیایی")]
        public decimal CityLongitude { get; set; }

        [Display(Name = "هاب لجستیکی")]
        public bool CityIsHub { get; set; }
    }
}
