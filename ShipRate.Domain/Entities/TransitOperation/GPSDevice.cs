using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class GPSDevice : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string GPSDeviceSerialNumber { get; set; } = string.Empty;
        public Guid GPSProviderId { get; set; }
        public int GPSDeviceStatusId { get; set; }
        public string Remarks { get; set; }
        //Relatiions
        public GPSProvider GPSProvider { get; set; }
        public GPSDeviceStatus GPSDeviceStatus { get; set; }
    }
}
