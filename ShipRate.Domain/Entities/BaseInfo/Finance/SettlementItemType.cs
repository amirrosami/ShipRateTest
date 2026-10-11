using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class SettlementItemType : SystemEntity
    {
        public string SettlementItemTypeName { get; set; }
        public string SettlementItemTypeLocalName { get; set; }
    }
}
