using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class HSCodeSnapshot : BusinessEntity
    {
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CountryId { get; set; }
        public DateTime CapturedAt { get; set; }

        //
        public Country Country { get; set; }
    }
}
