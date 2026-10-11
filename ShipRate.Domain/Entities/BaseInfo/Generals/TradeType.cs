using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class TradeType : SystemEntity
    {
        public int SortOrder { get; set; }
        public string TradeTypeName { get; set; } = string.Empty;
        public string TradeTypeLocalName { get; set; } = string.Empty;
    }
}



