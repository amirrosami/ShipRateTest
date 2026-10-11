using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractRate : BusinessEntity
    {
        public Guid ContractRouteId { get; set; }
        public Guid ContractContainerSizeTypeId { get; set; }
        public Guid ContractChargeTypeId { get; set; }
        public Guid? DGId { get; set; }
        public int? CntOwnTypeId { get; set; }
        public decimal RateValue { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }

        //Relations
        public ContractRoute? ContractRoute { get; set; }
        public ContractContainerSizeType? ContractContainerSizeType { get; set; }
        public ContractChargeType? ContractChargeType { get; set; }
        public DG? DG { get; set; }
        public CntOwnType? CntOwnType { get; set; }
    }
}
