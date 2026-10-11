using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class ContainerSize : SystemEntity
    {
        public string ContainerSizeName { get; set; } = string.Empty;
        public decimal TEUFactor { get; set; }
    }
}
