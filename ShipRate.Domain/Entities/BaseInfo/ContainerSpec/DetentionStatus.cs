using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class DetentionStatus : SystemEntity
    {
        public int Id { get; set; }
        public string DetentionStatusName { get; set; }
    }
}
