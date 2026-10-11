using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.JobSpec
{
    public class JobType : SystemEntity
    {
        public int SortOrder { get; set; }
        public string JobTypeName { get; set; }
        public string JobTypeLocalName { get; set; }
    }
}



//سناریوی کسب و کار 