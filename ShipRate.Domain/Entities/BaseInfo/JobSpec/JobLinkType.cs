using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.JobSpec
{
    public class JobLinkType : SystemEntity
    {
        public string JobLinkTypeName { get; set; } = string.Empty;
        public string JobLinkTypeLocalName { get; set; } = string.Empty;
    }
}
