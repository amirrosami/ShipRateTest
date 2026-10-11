using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Finance
{
    public class ChequeType : SystemEntity
    {
        public string ChequeTypeName { get; set; } = string.Empty;
        public string ChequeTypeLocalName { get; set; } = string.Empty;
    }
}
