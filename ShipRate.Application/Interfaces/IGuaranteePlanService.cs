using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IGuaranteePlanService
    {
        Task<PagedResultDto<GuaranteePlanDto>> GetPagedListAsync(GuaranteePlanSearchDto searchDto);
        Task<GuaranteePlanDto> GetByIdAsync(Guid id);
        Task<GuaranteePlanDto> CreateAsync(GuaranteePlanCreateOrUpdateDto dto, string userId);
        Task<GuaranteePlanDto> UpdateAsync(Guid id, GuaranteePlanCreateOrUpdateDto dto, string userId);
        Task DeleteAsync(Guid id);
        Task<List<SelectListItem>> GetCompanyLookupAsync();
        Task<List<SelectListItem>> GetInOutLookupAsync();
        Task<List<SelectListItem>> GetShippingLineLookupAsync();
        Task<List<SelectListItem>> GetCustomerLookupAsync();
        Task<List<BlLookupResultDto>> SearchBlsAsync(string term);
    }
}
