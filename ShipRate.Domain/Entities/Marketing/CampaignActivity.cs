using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Marketting;

namespace ShipRate.Domain.Entities.Marketing
{
    public class CampaignActivity : BusinessEntity
    {
        public Guid CampaignId { get; set; }
        public int? ActivityTypeId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public DateTime ActivityDate { get; set; }
        public decimal Cost { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Campaign Campaign { get; set; }
        public ActivityType ActivityType { get; set; }
    }
}
