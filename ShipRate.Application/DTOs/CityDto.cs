using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    public class CityDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "نام شهر الزامی است")]
        [StringLength(100, ErrorMessage = "نام شهر نباید بیشتر از 100 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "انتخاب کشور الزامی است")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفا کشور را انتخاب کنید")]
        public int CountryId { get; set; }

        public string? CountryName { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "جمعیت باید عددی مثبت باشد")]
        public long? Population { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "مساحت باید عددی مثبت باشد")]
        public decimal? Area { get; set; }

        public bool IsCapital { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
