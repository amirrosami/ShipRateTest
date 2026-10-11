using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.JobSpec
{
    public class JobStage : SystemEntity
    {
        public int SortOrder { get; set; }
        public string JobStageName { get; set; }
        public string JobStageLocalName { get; set; }
    }
}
