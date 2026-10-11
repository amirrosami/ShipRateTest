using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentLegBorder : BusinessEntity
    {
        public Guid ShipmentLegId { get; set; }
        public int SequenceNumber { get; set; }
        public Guid BorderId { get; set; }
        public DateTime? EntryDateTime { get; set; }
        public DateTime? ExitDateTime { get; set; }
        //Relations
        public ShipmentLeg ShipmentLeg { get; set; }
        public Location Border { get; set; }
    }
}
