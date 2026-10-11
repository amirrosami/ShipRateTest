using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Application.DTOs
{
    public class LeadDto
    {
        public Guid? Id { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Phone { get; set; }
        public Guid? IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public int? EstimatedRevenue { get; set; }
        public int? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        public string? InternalNotes { get; set; }
        public int? TradeTypeId { get; set; }
        public string? TradeTypeName { get; set; }
        public string? CargoType { get; set; }
        public int? LeadSourceId { get; set; }
        public string? LeadSourceName { get; set; }
        public int? LeadStatusId { get; set; }
        public string? LeadStatusName { get; set; }
        public int? LeadScore { get; set; }
        public int? LostReasonId { get; set; }
        public string? LostReasonName { get; set; }
        public string? LostReasonDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class LeadActivityDto
    {
        public Guid Id { get; set; }
        public Guid LeadId { get; set; }
        public string LeadCompanyName { get; set; } = string.Empty;
        public int? ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; } = string.Empty;
        public string ActivityTypeLocalName { get; set; } = string.Empty;
        public int? ActivityResultId { get; set; }
        public string ActivityResultName { get; set; } = string.Empty;
        public string ActivityResultLocalName { get; set; } = string.Empty;
        public int? ActivityOutcomeId { get; set; }
        public string ActivityOutcomeName { get; set; } = string.Empty;
        public string ActivityOutcomeLocalName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class LeadSearchDto
    {
        public string SearchTerm { get; set; }
        public int? LeadStatusId { get; set; }
        public int? LeadSourceId { get; set; }
        public DateTime? ActivityDateFrom { get; set; }
        public DateTime? ActivityDateTo { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public int? MinLeadScore { get; set; }
        public Guid? IndustryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class AssignUserDto
    {
        public Guid LeadId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public Guid? CurrentAssignedUserId { get; set; }
        public Guid? SelectedUserId { get; set; }
        public List<SelectListItem> Users { get; set; } = new();
    }

}
