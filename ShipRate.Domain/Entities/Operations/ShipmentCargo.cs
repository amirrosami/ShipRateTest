using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BillOfLading;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentCargo : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public Guid? ShipmentContainerId { get; set; }
        public Guid CargoDetailId { get; set; }   ////Job cargo details
        public int AllocatedPackageCounts { get; set; }
        public decimal AllocatedGrossWeight { get; set; }
        public decimal AllocatedVolume { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public Shipment Shipment { get; set; }
        public ShipmentContainer ShipmentContainer { get; set; }
        public CargoDetail CargoDetail { get; set; }
    }
}
