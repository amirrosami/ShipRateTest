using ShipRate.Domain.Enums;
using ShipRate.Domain.Extensions;

namespace ShipRate.Application.DTOs.Locations
{
    public class LocationListItemDto
    {
        public Guid Id { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public EnumLocationType LocationType { get; set; }
        public string LocationTypeDisplay => LocationType.GetDisplayName();
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
