using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class ContainerType : SystemEntity
    {
        public string ContainerTypeName { get; set; } = string.Empty;
        public string ContainerTypeAbb { get; set; } = string.Empty;
        public bool isSpecial { get; set; }
        public bool isRefer { get; set; }
    }
}
