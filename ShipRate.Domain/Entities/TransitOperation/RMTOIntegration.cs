using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class RMTOIntegration : BusinessEntity
    {
        public Guid JobId { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid ShipmentLegId { get; set; }
        public string RMTOBillNo { get; set; }
        public string TrackingCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastSyncedAt { get; set; }
        public string PayLoad { get; set; }   //NVarchar(MAX) -JSON

        //Relations
        public Job Job { get; set; }
        public Shipment Shipment { get; set; }
        public ShipmentLeg ShipmentLeg { get; set; }
    }
}
