using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Marketting
{
    public class CampaignStatus : SystemEntity
    {
        public string CampaignStatusName { get; set; }
        public string CampaignStatusLocalName { get; set; }
    }
}
