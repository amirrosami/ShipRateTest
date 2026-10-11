using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Marketing
{
    public class Campaign : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public int CampaignTypeId { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public DateTime CampaignEndDate { get; set; }

        public decimal CampaignBudget { get; set; }
        public decimal ActualCost { get; set; }

        public decimal ExpectedRevenue { get; set; }
        public decimal ActualRevenue { get; set; }

        public int CampaignStatusId { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public LeadSource CampaignType { get; set; }
        public LeadSource CampaignStatus { get; set; }
        public Company Company { get; set; }
        public ApplicationUser OwnerUser { get; set; }

        public ICollection<CampaignMember> Members { get; set; }
        public ICollection<CampaignActivity> Activities { get; set; }
    }
}
