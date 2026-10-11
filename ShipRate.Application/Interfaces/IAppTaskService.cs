using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.AppTasks;

namespace ShipRate.Application.Interfaces
{
    public interface IAppTaskService
    {
        Task<AppTaskIndexViewModel> GetIndexDataAsync(
            AppTaskFilterDto filter,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<AppTaskListItemDto>> SearchAsync(
            AppTaskFilterDto filter,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<CreateAppTaskViewModel> GetCreateDataAsync(
            int? taskId,
            int? entityTypeId,
            Guid? relatedEntityId,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            CreateAppTaskDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task<AppTaskDetailsDto> GetDetailsAsync(
            Guid id,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task AssignAsync(
            AssignAppTaskDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task AddCommentAsync(
            AddAppTaskCommentDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task CompleteAsync(
            CompleteAppTaskDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default);

        Task ToggleCheckListItemAsync(
            ToggleAppTaskChecklistDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default);
        Task StartAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default);


        Task<List<AppTaskDashboardItemDto>> GetDashboardTasksAsync(
            Guid currentUserId,
            string filter,
            string? keyword,
            int take = 5);


    }
}
