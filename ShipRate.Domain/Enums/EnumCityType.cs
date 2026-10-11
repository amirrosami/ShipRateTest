using System.ComponentModel.DataAnnotations;

namespace ShipRate.Domain.Enums
{
    public enum EnumCityType
    {
        [Display(Name = "شهر")]
        City = 1,

        [Display(Name = "بندر")]
        Port = 2,

        [Display(Name = "شهر و بندر")]
        CityAndPort = 3,
    }
}
