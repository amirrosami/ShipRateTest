using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class ClaimStatus : SystemEntity
    {
        public string ClaimStatusName { get; set; }
        public string ClaimStatusLocalName { get; set; }
    }
}
