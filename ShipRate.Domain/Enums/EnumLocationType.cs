using System.ComponentModel.DataAnnotations;

namespace ShipRate.Domain.Enums
{
    public enum EnumLocationType
    {
        [Display(Name = "استان")]
        Province = 1,

        [Display(Name = "شهر/بندر")]
        CityOrPort = 2,

        [Display(Name = "فرودگاه")]
        AirPort = 3,

        [Display(Name = "مرز")]
        Border = 4,

        [Display(Name = "گمرک")]
        CustomOffice = 5,

        [Display(Name = "ترمینال")]
        Terminal = 6,

        [Display(Name = "انبار/دپو")]
        Depot = 7,

        [Display(Name = "منطقه آزاد")]
        FreeZone = 8
    }
}
