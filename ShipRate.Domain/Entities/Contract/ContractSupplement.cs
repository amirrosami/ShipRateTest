using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractSupplement : BusinessEntity
    {
        public Guid ContractId { get; set; }
        public Guid FACContractId { get; set; }

        //Relations
        public Contract Contract { get; set; }
        public FACContract FACContract { get; set; }
    }
}
