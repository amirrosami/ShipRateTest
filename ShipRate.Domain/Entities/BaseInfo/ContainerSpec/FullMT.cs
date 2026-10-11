using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class FullMT : SystemEntity
    {
        public string FullMTCode { get; set; } = string.Empty;  //4=Empty Or 5=Full
        public string FullMTName { get; set; } = string.Empty;
    }
}
