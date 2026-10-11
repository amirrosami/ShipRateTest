using ShipRate.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;
namespace ShipRate.Domain.Entities.BaseInfo.Locations
{
    public class Zone : SystemEntity
    {
        [StringLength(100)]
        public string ZoneName { get; set; } = string.Empty;
        public string ZoneLocalName { get; set; } = string.Empty;
        public IEnumerable<Country>? Countries { get; set; }
    }
}