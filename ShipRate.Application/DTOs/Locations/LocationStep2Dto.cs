using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class LocationStep2Dto
    {
        // ─── اطلاعات منتقل‌شده از Step 1 ───
        public int CountryId { get; set; }
        public EnumLocationType LocationType { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;

        // ─── فیلدهای تخصصی ───
        [StringLength(100)]
        [Display(Name = "نام استان")]
        public string? ProvinceName { get; set; }

        [StringLength(3, MinimumLength = 3, ErrorMessage = "کد IATA باید دقیقاً 3 کاراکتر باشد")]
        [Display(Name = "کد IATA")]
        public string? IataCode { get; set; }

        [StringLength(4, MinimumLength = 4, ErrorMessage = "کد ICAO باید دقیقاً 4 کاراکتر باشد")]
        [Display(Name = "کد ICAO")]
        public string? IcaoCode { get; set; }

        [StringLength(200)]
        [Display(Name = "آدرس")]
        public string? Address { get; set; }

        [StringLength(20)]
        [Display(Name = "تلفن")]
        public string? Phone { get; set; }

        [StringLength(500)]
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
    }
}
