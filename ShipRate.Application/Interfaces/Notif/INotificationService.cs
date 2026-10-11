using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Notif;

namespace ShipRate.Application.Interfaces.Notif
{
    public interface INotificationService
    {
        Task<CreateNotificationDto> GetCreateModelAsync(
            CancellationToken cancellationToken = default);

        Task<List<SelectListItemDto>> GetUsersByTypeAsync(
            int selectedUserTypeId,
            CancellationToken cancellationToken = default);

        Task<ServiceResultDto<Guid>> CreateManualAsync(
            CreateManualNotificationRequest request,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<NavbarNotificationsViewModel> GetNavbarNotificationsAsync(
            Guid userId,
            int take = 5,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<NotificationListItemDto>> GetUserNotificationsAsync(
            Guid userId,
            NotificationListFilterDto filter,
            CancellationToken cancellationToken = default);

        Task<ServiceResultDto> MarkAsReadAsync(
            Guid userNotificationId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<ServiceResultDto> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken = default);



        Task<NotificationAdminListViewModel> GetAdminListAsync(
            NotificationAdminFilterDto filter,
            CancellationToken ct = default);

        Task<NotificationDetailsDto?> GetDetailsAsync(
            Guid notificationId,
            CancellationToken ct = default);

        Task<ServiceResultDto> ArchiveAsync(
            Guid notificationId,
            Guid currentUserId,
            CancellationToken ct = default);

        Task<CreateManualNotificationRequest?> BuildDuplicateAsync(
            Guid notificationId,
            CancellationToken ct = default);
        Task CreateForActivityLogAsync(Guid activityLogId, CancellationToken ct = default);
    }
}
