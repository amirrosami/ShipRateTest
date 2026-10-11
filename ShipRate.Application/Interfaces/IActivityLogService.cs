using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface IActivityLogService
    {
        Task<Guid> CreateAsync(CreateActivityLogRequest request, CancellationToken cancellationToken = default);

        Task LogEventAsync(CreateActivityLogRequest request, CancellationToken cancellationToken = default);

        Task LogStatusChangeAsync(CreateActivityLogRequest request, CancellationToken cancellationToken = default);

        Task LogHistoryAsync(CreateActivityLogRequest request, CancellationToken cancellationToken = default);

        Task LogAuditAsync(CreateActivityLogRequest request, CancellationToken cancellationToken = default);

        Task<List<ActivityLogListItemVm>> GetEntityTimelineAsync(
            int entityTypeId,
            Guid entityId,
            CancellationToken cancellationToken = default);


        // متدهای جدید اضافه شده برای مدیریت رویدادها از سمت کنترلر
        Task<ActivityLogDetailsVm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateActivityLogRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<EventTypeLookupVm>> GetEventTypesLookupAsync(int entityTypeId, CancellationToken cancellationToken = default);
        Task<List<RecentActivityViewModel>> GetRecentActivitiesAsync(int take = 8, CancellationToken ct = default);
    }









}
