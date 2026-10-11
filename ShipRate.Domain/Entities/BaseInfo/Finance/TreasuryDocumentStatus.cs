using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class TreasuryDocumentStatus : SystemEntity
    {
        public string TreasuryDocumentStatusName { get; set; }
        public string TreasuryDocumentStatusLocalName { get; set; }
    }
}
