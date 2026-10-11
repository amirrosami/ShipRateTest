using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class ContainerDamageArea : SystemEntity
    {
        public string ContainerDamageAreaName { get; set; } = string.Empty;
    }
}

// Door, Roof, SidePanel, Floor, CornerPost
