using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class DepotDto : LocationBaseDto
    {
        [Required(ErrorMessage = "انتخاب شهر الزامی است")]
        [Display(Name = "شهر")]
        public Guid CityId { get; set; }
        public string? CityName { get; set; }

        [Required(ErrorMessage = "نوع انبار الزامی است")]
        [Display(Name = "نوع انبار")]
        public EnumDepotType DepotType { get; set; }

        [Required(ErrorMessage = "آدرس الزامی است")]
        [StringLength(500, ErrorMessage = "آدرس نباید بیشتر از 500 کاراکتر باشد")]
        [Display(Name = "آدرس")]
        public string Address { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "عرض جغرافیایی باید بین -90 تا 90 باشد")]
        [Display(Name = "عرض جغرافیایی")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "طول جغرافیایی باید بین -180 تا 180 باشد")]
        [Display(Name = "طول جغرافیایی")]
        public decimal? Longitude { get; set; }
    }
}
