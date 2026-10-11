using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class ChargeStatus : SystemEntity
    {
        public string ChargeStatusName { get; set; }
        public string ChargeStatusLocalName { get; set; }
    }
}
