using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Agents
{
    public class TerritoryCity : BusinessEntity
    {
        public Guid TerritoryId { get; set; }
        public Guid CityId { get; set; }

        //Relations
        public Territory Territory { get; set; }
        public Location City { get; set; }
    }
}
