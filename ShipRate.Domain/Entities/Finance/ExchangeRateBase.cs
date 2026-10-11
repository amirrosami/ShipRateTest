using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Finance
{
    public class ExchangeRateBase : SystemEntity
    {
        public string ExchangeRateBaseName { get; set; } = string.Empty;

    }
}
