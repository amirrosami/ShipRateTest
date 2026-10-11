using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class VGMStatus : SystemEntity
    {
        public string StatusName { get; set; }
        public string StatusLocalName { get; set; }
    }
}
