using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.RateQuotation
{
    public class BuyQuoteItem : BusinessEntity
    {
        public Guid BuyQuoteId { get; set; }
        public int ChargeTypeId { get; set; }
        public int? ContainerSizeTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public BuyQuote BuyQuote { get; set; }
        public ContainerSizeType? ContainerSizeType { get; set; }
        public ChargeType ChargeType { get; set; }
        public Currency Currency { get; set; }

    }
}
