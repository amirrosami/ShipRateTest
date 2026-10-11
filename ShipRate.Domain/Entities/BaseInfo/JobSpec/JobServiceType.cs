using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.JobSpec
{
    public class ServiceType : SystemEntity
    {
        public int SortOrder { get; set; }
        public string? ServiceTypeCode { get; set; } = string.Empty;
        public string ServiceTypeName { get; set; }
        public string ServiceTypeLocalName { get; set; }
    }
}
