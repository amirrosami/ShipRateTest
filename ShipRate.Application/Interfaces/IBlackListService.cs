using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IBlackListService
    {
        Task<PagedResultDto<BlackListDto>> GetPagedListAsync(BlackListSearchDto filter);
        Task<BlackListDto?> GetByIdAsync(Guid id);
        Task CreateAsync(CreateBlackListDto dto, Guid currentUserId);
        Task ExitAsync(ExitBlackListDto dto, Guid currentUserId);
        Task DeleteAsync(Guid id);
        Task<SelectList> GetPeopleSelectListAsync();
        Task<SelectList> GetReasonsSelectListAsync();
    }
}
