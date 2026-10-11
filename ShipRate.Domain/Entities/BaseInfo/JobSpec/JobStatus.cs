using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.JobSpec
{
    public class JobStatus : SystemEntity
    {
        public int SortOrder { get; set; }
        public string JobStatusName { get; set; }
        public string JobStatusLocalName { get; set; }
    }
}
