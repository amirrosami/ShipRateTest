using ShipRate.Domain.Entities.Base;
namespace ShipRate.Domain.Entities.BaseInfo.Locations
{
    public class Country : SystemEntity
    {
        public int? ZoneId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryLocalName { get; set; } = string.Empty;
        public string CountryIsoCode2 { get; set; } = string.Empty;
        public string? CountryIsoCode3 { get; set; } = string.Empty;
        public string? CountryPhoneCode { get; set; } = string.Empty;
        public string? CustomsCode { get; set; } = string.Empty;
        public string? TimeZone { get; set; } = string.Empty;
        public int? NumericCode { get; set; }
        public int? DefaultCurrencyId { get; set; }
        //Navigation
        public Zone? Zone { get; set; }
    }
}