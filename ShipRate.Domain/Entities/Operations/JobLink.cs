using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.JobSpec;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobLink : BusinessEntity
    {
        public Guid SourceJobId { get; set; }
        public Guid TargetJobId { get; set; }
        public int JobLinkTypeId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Job SourceJob { get; set; }
        public Job TargetJob { get; set; }
        public JobLinkType JobLinkType { get; set; }

    }
}
