using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class ChequeStatus : SystemEntity
    {
        public string ChequeStatusName { get; set; } = string.Empty;
        public string ChequeStatusLocalName { get; set; } = string.Empty;
    }
}
