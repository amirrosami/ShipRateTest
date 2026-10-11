using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class GPSRecharge : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid GPSDeviceId { get; set; }
        public Guid GPSProviderId { get; set; }
        public DateTime RechargeStartDate { get; set; }
        public DateTime RechargeEndDate { get; set; }
        public int GPSRechargeStatusId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Company Company { get; set; }
        public GPSDevice GPSDevice { get; set; }
        public GPSProvider GPSProvider { get; set; }
    }
}
