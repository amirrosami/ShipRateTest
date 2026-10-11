using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Locations
{
    public class TerminalDto : LocationBaseDto
    {
        [Required(ErrorMessage = "انتخاب شهر الزامی است")]
        [Display(Name = "شهر")]
        public Guid CityId { get; set; }
        public string? CityName { get; set; }

        [StringLength(500, ErrorMessage = "آدرس نباید بیشتر از 500 کاراکتر باشد")]
        [Display(Name = "آدرس")]
        public string? Address { get; set; }
    }
}
