using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class CntOwnLeaseType : SystemEntity
    {
        public string CntOwnLeaseTypeName { get; set; } = string.Empty;
        public string CntOwnLeaseTypeLocalName { get; set; } = string.Empty;
    }
}
