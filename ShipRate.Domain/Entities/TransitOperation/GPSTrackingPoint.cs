using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class GPSTrackingPoint : BusinessEntity
    {
        public Guid GPSDeviceId { get; set; }
        public Guid TruckId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Speed { get; set; }
        public DateTime RecordedAt { get; set; }


        //Relations
        public GPSDevice GPSDevice { get; set; }
        public Truck Truck { get; set; }
    }
}
