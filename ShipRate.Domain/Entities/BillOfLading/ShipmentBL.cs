using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class ShipmentBL : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public Guid BLId { get; set; }

        //Relations
        public Shipment Shipment { get; set; }
        public BL BL { get; set; }
    }
}
