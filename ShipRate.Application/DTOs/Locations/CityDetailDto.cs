namespace ShipRate.Application.DTOs.Locations
{
    public class CityDetailDto
    {
        public Guid Id { get; set; }
        public string LocationName { get; set; } = string.Empty;  // از Location
        public string LocationCode { get; set; } = string.Empty;
        public string CityIsoCode { get; set; } = string.Empty;
        public string CityTypeName { get; set; } = string.Empty;  // از Enum

        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;

        public Guid? ProvinceId { get; set; }
        public string? ProvinceName { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsHub { get; set; }

        // فقط برای بنادر
        public Guid? PortCityId { get; set; }
        public string? PortCityName { get; set; }

        // آمار
        public int AirPortCount { get; set; }
        public int TerminalCount { get; set; }
        public int DepotCount { get; set; }
    }

    // DTO برای Autocomplete / Dropdown
    public class LocationLookupDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string DisplayText => $"{Code} - {Name} ({CountryName})";
    }
}
