using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    public class CountryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کشور الزامی است")]
        [StringLength(100, ErrorMessage = "نام کشور نباید بیشتر از 100 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "کد کشور الزامی است")]
        [StringLength(10, ErrorMessage = "کد کشور نباید بیشتر از 10 کاراکتر باشد")]
        public string Code { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "نام پایتخت نباید بیشتر از 100 کاراکتر باشد")]
        public string? Capital { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "جمعیت باید عددی مثبت باشد")]
        public long? Population { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "مساحت باید عددی مثبت باشد")]
        public decimal? Area { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int CitiesCount { get; set; }
    }
}
