using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Entities.Security;
namespace ShipRate.Domain.Entities.Marketing
{
    public class Lead : BusinessEntity
    {
        public string? CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; } = string.Empty;
        public string? EmailAddress { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Guid? IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public int? EstimatedRevenue { get; set; }
        public int? CurrencyId { get; set; }

        public Guid? AssignedToUserId { get; set; }
        public string? InternalNotes { get; set; }

        public int? TradeTypeId { get; set; }
        public string? CargoType { get; set; } = string.Empty;
        public int LeadSourceId { get; set; }
        public int LeadStatusId { get; set; }
        public int? LeadScore { get; set; }
        public int? LostReasonId { get; set; }
        public string? LostReasonDescription { get; set; } = string.Empty;

        public Guid? ConvertedToPersonId { get; set; }


        //Relations
        public TradeType? TradeType { get; set; }
        public LeadSource? LeadSource { get; set; }
        public LeadStatus? LeadStatus { get; set; }
        public LostReason? LostReason { get; set; }
        public Industry? Industry { get; set; }
        public Currency? Currency { get; set; }
        public ApplicationUser? AssignedToUser { get; set; }
    }
}