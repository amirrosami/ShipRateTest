using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Operations;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class GPSAssignment : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid GPSDeviceId { get; set; }
        public Guid TruckId { get; set; }
        public DateTime InstalledAt { get; set; }
        public DateTime RemovedAt { get; set; }
        public int GPSAssignmentStatusId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public GPSDevice GPSDevice { get; set; }
        public Truck Truck { get; set; }
    }
}
