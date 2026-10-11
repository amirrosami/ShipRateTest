using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface ILeadService
    {
        // Leads
        Task<PagedResultDto<LeadDto>> GetLeadsAsync(LeadSearchDto filter);
        Task<LeadDto> GetLeadAsync(Guid id);
        Task<LeadDto> CreateLeadAsync(LeadDto dto, Guid userId);
        Task<LeadDto> UpdateLeadAsync(LeadDto dto, Guid userId);
        Task DeleteLeadAsync(Guid id);

        // Assign
        Task<AssignUserDto> GetAssignModelAsync(Guid leadId);
        Task AssignLeadAsync(Guid leadId, Guid userId, Guid performedByUserId);

        // Lead Activities
        Task<List<LeadActivityDto>> GetActivitiesAsync(Guid leadId);
        Task<LeadActivityDto> GetActivityAsync(Guid id);
        Task<LeadActivityDto> CreateActivityAsync(LeadActivityDto dto, Guid userId);
        Task<LeadActivityDto> UpdateActivityAsync(LeadActivityDto dto, Guid userId);
        Task DeleteActivityAsync(Guid id);

        // Dropdown Providers
        Task<List<SelectListItem>> GetLeadStatusesAsync();
        Task<List<SelectListItem>> GetLeadSourcesAsync();
        Task<List<SelectListItem>> GetIndustriesAsync();
        Task<List<SelectListItem>> GetCurrenciesAsync();
        Task<List<SelectListItem>> GetTradeTypesAsync();
        Task<List<SelectListItem>> GetLostReasonsAsync();
        Task<List<SelectListItem>> GetUsersAsync();
        Task<List<SelectListItem>> GetActivityTypesAsync();
        Task<List<SelectListItem>> GetActivityResultsAsync();
        Task<List<SelectListItem>> GetActivityOutcomesAsync();
    }
}
