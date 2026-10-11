using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Operations
{
    public class Shipment : BusinessEntity
    {
        public Guid JobId { get; set; }
        public string ShipmentTitle { get; set; }
        public int ShipmentStatusId { get; set; }

        //Relations
        public ShipmentStatus ShipmentStatus { get; set; }
        public Job Job { get; set; }
        public List<ShipmentLeg> ShipmentLegs { get; set; }
    }
}
