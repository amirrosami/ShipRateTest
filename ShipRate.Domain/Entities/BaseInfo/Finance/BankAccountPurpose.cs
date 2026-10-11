using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class BankAccountPurpose : SystemEntity
    {
        public string BankAccountPurposeName { get; set; } = string.Empty;
        public string BankAccountPurposeLocalName { get; set; } = string.Empty;
    }
}
