using ShipRate.Domain.Enums;
using ShipRate.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class LocationDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "انتخاب کشور الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفا کشور را انتخاب کنید")]
        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        [Required(ErrorMessage = "کد مکان الزامی است")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "کد مکان باید بین 2 تا 10 کاراکتر باشد")]
        public string LocationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "نام مکان الزامی است")]
        [StringLength(150, ErrorMessage = "نام مکان نباید بیشتر از 150 کاراکتر باشد")]
        public string LocationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "نوع مکان الزامی است")]
        public EnumLocationType LocationType { get; set; }

        public string? LocationTypeDisplay => LocationType.GetDisplayName();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ─── جزئیات تکمیلی (Step 2 Wizard) ───────────────────────────────────

        // برای City / Port
        public string? ProvinceName { get; set; }

        // برای Airport
        [StringLength(3, ErrorMessage = "کد IATA باید 3 کاراکتر باشد")]
        public string? IataCode { get; set; }

        [StringLength(4, ErrorMessage = "کد ICAO باید 4 کاراکتر باشد")]
        public string? IcaoCode { get; set; }

        // برای Terminal / Depot
        [StringLength(200, ErrorMessage = "آدرس نباید بیشتر از 200 کاراکتر باشد")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "شماره تماس نباید بیشتر از 20 کاراکتر باشد")]
        public string? Phone { get; set; }

        // برای همه انواع (اختیاری)
        [StringLength(500, ErrorMessage = "توضیحات نباید بیشتر از 500 کاراکتر باشد")]
        public string? Description { get; set; }
    }
}
