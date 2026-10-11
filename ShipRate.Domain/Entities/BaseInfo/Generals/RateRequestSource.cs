using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class RateRequestSource : SystemEntity
    {
        public string RateRequestSourceName { get; set; } = string.Empty;
        public string RateRequestSourceLocalName { get; set; } = string.Empty;
    }
}
