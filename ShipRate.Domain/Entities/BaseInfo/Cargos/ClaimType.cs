using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class ClaimType : SystemEntity
    {
        public string ClaimTypeName { get; set; }
        public string ClaimTypeLocalName { get; set; }
    }
}
