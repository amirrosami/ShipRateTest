using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class ProofOfDelivery : BusinessEntity
    {
        public Guid CMRId { get; set; }
        public Guid ShipmentId { get; set; }
        public Guid TruckId { get; set; }
        public DateTime DeliveredAt { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Guid GPSTrackId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public byte[]? ReceiverSignature { get; set; }
        public string DeliveryNote { get; set; } = string.Empty;
    }
}
