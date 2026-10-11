using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;

namespace ShipRate.Domain.Entities.Finance
{
    public class ExchangeRate : BusinessEntity
    {
        public int ExchangeRateBaseId { get; set; }
        public DateTime ExchangeRateDate { get; set; }
        public DateTime ExchangeRateValidToDate { get; set; }
        public string ExchangeRateSource { get; set; } = string.Empty;
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal ExchangeRateValue { get; set; }

        //Relations
        public ExchangeRateBase? ExchangeRateBase { get; set; }
        public Currency? FromCurrency { get; set; }
        public Currency? ToCurrency { get; set; }
    }
}
