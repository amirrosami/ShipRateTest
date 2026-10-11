using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Rate;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractRoute : BusinessEntity
    {
        public Guid ContractId { get; set; }
        public Guid? RouteId { get; set; }
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }

        //Relation
        public Contract? Contract { get; set; }
        public Route? Route { get; set; }
        public Location? Origin { get; set; }
        public Location? Destination { get; set; }
        public List<ContractRate>? ContractRates { get; set; }
    }
}
