using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractChargeType : BusinessEntity
    {
        public Guid ContractId { get; set; }
        public int ChargeTypeId { get; set; }
        public int CalculationById { get; set; }

        //Relations
        public Contract? Contract { get; set; }
        public ChargeType? ChargeType { get; set; }
        public List<ContractRate>? ContractRates { get; set; }
        public CalculationBy CalculationBy { get; set; }
    }
}
