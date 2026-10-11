namespace ShipRate.Application.Base
{
    public class LocationListDto
    {
        public Guid LocationId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string? LocationTypeName { get; set; }
        public string? CountryName { get; set; }
        public string? CityTypeName { get; set; }
        public string? ParentLocationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
