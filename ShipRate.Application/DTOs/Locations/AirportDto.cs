using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class AirportDto : LocationBaseDto
    {
        [Required(ErrorMessage = "انتخاب شهر الزامی است")]
        [Display(Name = "شهر")]
        public Guid CityId { get; set; }
        public string? CityName { get; set; }

        [Required(ErrorMessage = "کد IATA الزامی است")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "کد IATA باید دقیقاً 3 کاراکتر باشد")]
        [Display(Name = "کد IATA")]
        public string IATACode { get; set; } = string.Empty;

        [Required(ErrorMessage = "کد ICAO الزامی است")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "کد ICAO باید دقیقاً 4 کاراکتر باشد")]
        [Display(Name = "کد ICAO")]
        public string ICAOCode { get; set; } = string.Empty;
    }
}
