using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class TreasuryDocumentType : SystemEntity
    {
        public string TreasuryDocumentTypeName { get; set; }
        public string TreasuryDocumentTypeLocalName { get; set; }
    }
}
