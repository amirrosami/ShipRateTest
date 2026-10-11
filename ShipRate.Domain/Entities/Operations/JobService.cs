using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobService : BusinessEntity
    {
        public Guid JobId { get; set; }
        public int ServiceTypeId { get; set; }
        public Guid VendorId { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Remarks { get; set; }
        //Relations
        public Job Job { get; set; }
        public ServiceType ServiceType { get; set; }
        public Person Vendor { get; set; }
    }
}


