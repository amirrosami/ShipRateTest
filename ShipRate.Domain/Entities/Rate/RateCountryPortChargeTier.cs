using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Rate
{
    public class RateCountryPortChargeTier : BusinessEntity
    {
        public Guid RateCountryPortChargeId { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
        public decimal Amount { get; set; }
        public bool IsPer { get; set; }
        public bool IsFixed { get; set; }
        //Relations
        public RateCountryPortCharge? RateCountryPortCharge { get; set; }
    }
}
