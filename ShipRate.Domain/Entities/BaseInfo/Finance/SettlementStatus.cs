using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class SettlementStatus : SystemEntity
    {
        public string SettlementStatusName { get; set; }
        public string SettlementStatusLocalName { get; set; }
    }
}
