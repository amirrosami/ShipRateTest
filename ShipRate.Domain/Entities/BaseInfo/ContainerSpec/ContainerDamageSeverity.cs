using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class ContainerDamageSeverity : SystemEntity
    {
        public string ContainerDamageSeverityName { get; set; }
    }
}
// Minor, Major, Critical