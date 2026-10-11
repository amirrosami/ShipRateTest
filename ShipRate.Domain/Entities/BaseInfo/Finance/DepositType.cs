using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class DepositType : SystemEntity
    {
        public string DepositTypeName { get; set; } = string.Empty;
    }
}
