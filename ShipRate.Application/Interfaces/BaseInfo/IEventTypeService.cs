using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IEventTypeService
    {
        Task<List<EventTypeCategoryDto>> GetCategoriesAsync();
        Task<PagedResultDto<EventTypeListItemDto>> GetPagedAsync(EventTypeSearchDto input);
        Task<CreateEventTypeDto> GetCreateDtoAsync(int? categoryId);
        Task<UpdateEventTypeDto?> GetUpdateDtoAsync(int id);
        Task<EventTypeFormViewModel> GetCreateFormAsync(int? categoryId);
        Task<EventTypeFormViewModel?> GetEditFormAsync(int id);
        Task<(bool Success, string Message)> CreateAsync(CreateEventTypeDto dto);
        Task<(bool Success, string Message)> UpdateAsync(UpdateEventTypeDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
