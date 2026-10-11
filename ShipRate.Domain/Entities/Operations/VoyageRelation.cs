using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Operations
{
    public class VoyageRelation : BusinessEntity
    {
        public Guid PortId { get; set; }
        public Guid? TerminalId { get; set; }
        public Guid InboundVoyageId { get; set; }
        public Guid OutboundVoyageId { get; set; }
        public int PortOperationTypeId { get; set; }

        //Relations
        public Location Port { get; set; }
        public Location? Terminal { get; set; }
        public Voyage InboundVoyage { get; set; }
        public Voyage OutboundVoyage { get; set; }
        public PortOperationType PortOperationType { get; set; }
    }
}
