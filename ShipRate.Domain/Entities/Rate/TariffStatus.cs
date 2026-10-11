using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Rate
{
    public class TariffStatus : SystemEntity
    {
        public string TariffStatusName { get; set; } = string.Empty;
    }
}
