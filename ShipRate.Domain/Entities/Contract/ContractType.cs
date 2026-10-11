using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Contract
{
    public class ContractType : SystemEntity
    {
        public string ContractTypeName { get; set; } = string.Empty;
    }
}
