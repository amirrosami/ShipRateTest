using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Rate
{
    public class RateCountryPortCharge : BusinessEntity
    {
        public Guid RateCountryPortId { get; set; }
        public int ChargeTypeId { get; set; }
        public int CalculationById { get; set; }
        public int CalculationTypeId { get; set; }
        public decimal CurrencyId { get; set; }
        public decimal? Amount { get; set; }

        //Relations
        public RateCountryPort? RateCountryPort { get; set; }
        public ChargeType ChargeType { get; set; }
        public CalculationBy CalculationBy { get; set; }
        public CalculationType CalculationType { get; set; }
        public Currency Currency { get; set; }
    }
}
