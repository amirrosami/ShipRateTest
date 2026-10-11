using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class CampaignType : SystemEntity
    {
        public string CampaignTypeName { get; set; }
        public string CampaignTypeLocalName { get; set; }
    }
}
