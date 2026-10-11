using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Notif;

namespace ShipRate.Application.Interfaces.Notif
{
    public interface INotificationRuleService
    {
        Task<NotificationRuleDto?> GetRuleForActivityAsync(
            string activityType,
            int entityTypeId,
            CancellationToken ct = default);


        Task<PagedResultDto<NotificationRuleIndexDto>> GetListAsync(NotificationRuleGetListInput input);
        Task<NotificationRuleCreateDto> GetCreateDtoAsync();
        Task CreateAsync(NotificationRuleCreateDto input);
        Task<NotificationRuleEditDto?> GetEditDtoAsync(Guid id);
        Task UpdateAsync(NotificationRuleEditDto input);
        Task<NotificationRuleDetailsDto?> GetDetailsAsync(Guid id);
        Task<NotificationRuleDeleteDto?> GetDeleteDtoAsync(Guid id);
        Task DeleteAsync(Guid id);



    }
}
