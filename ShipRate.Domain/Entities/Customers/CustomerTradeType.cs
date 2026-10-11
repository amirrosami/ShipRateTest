using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerTradeType : BusinessEntity
    {
        public Guid CustomerId { get; set; }
        public int TradeTypeId { get; set; }

        //Relations
        public Customer Customer { get; set; }
        public TradeType TradeType { get; set; }
    }
}
