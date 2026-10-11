using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class CntOwnType : SystemEntity
    {
        public string CntOwnTypeName { get; set; } = string.Empty;
        public string CntOwnTypeDesc { get; set; } = string.Empty;
    }
}
