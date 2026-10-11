using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Cargo
{
    public class CargoCommodityDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "CargoCommodityName is required")]
        [MaxLength(250)]
        [Display(Name = "نام کالا")]
        public string CargoCommodityName { get; set; }

        [Required(ErrorMessage = "CargoCommodityLocalName is required")]
        [MaxLength(250)]
        [Display(Name = "نام محلی کالا")]
        public string CargoCommodityLocalName { get; set; }


        public bool IsActive { get; set; } = false;

    }
}
