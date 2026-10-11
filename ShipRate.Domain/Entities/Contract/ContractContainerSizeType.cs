using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
namespace ShipRate.Domain.Entities.Contract
{
    public class ContractContainerSizeType : BusinessEntity
    {
        public Guid ContractId { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public int? ContainerSizeTypeCounts { get; set; }
        //Relations
        public Contract? Contract { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
        public List<ContractRate>? ContractRates { get; set; }
    }
}