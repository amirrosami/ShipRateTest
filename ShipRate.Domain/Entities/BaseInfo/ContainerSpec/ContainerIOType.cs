using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class ContainerIOType : SystemEntity
    {
        public string ContainerIOTypeName { get; set; } = string.Empty;
        public int InOutId { get; set; }


        //relations
        public InOut InOut { get; set; }
    }
}