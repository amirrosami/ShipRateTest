using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.AppTasks
{
    public class AppTaskFilterDto
    {
        public string? Keyword { get; set; }
        public int? TaskStatusId { get; set; }
        public int? PriorityId { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public Guid? OwnerUserId { get; set; }
        public int? EntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }

        public bool? MyTasksOnly { get; set; }
        public bool? IncludeCompleted { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateAppTaskDto
    {
        public int? TaskId { get; set; }

        public int? EntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }

        public Guid? OwnerUserId { get; set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? TaskDesciption { get; set; }

        public int? PriorityId { get; set; }
        public int? SourceTypeId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public List<Guid> AssignedToUserIds { get; set; } = new();

        public List<CreateAppTaskChecklistItemDto> CheckLists { get; set; } = new();
    }

    public class CreateAppTaskChecklistItemDto
    {
        [Required]
        [MaxLength(250)]
        public string CheckListItemTitle { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsMandatory { get; set; }
        public int SortOrder { get; set; }
    }

    public class AssignAppTaskDto
    {
        [Required]
        public Guid AppTaskId { get; set; }

        [Required]
        public Guid AssignedToUserId { get; set; }

        public string? Notes { get; set; }
    }

    public class AddAppTaskCommentDto
    {
        [Required]
        public Guid AppTaskId { get; set; }

        [Required]
        [MaxLength(4000)]
        public string CommentText { get; set; } = string.Empty;

        public int CommentTypeId { get; set; } = 1;

        public bool IsInternal { get; set; }
    }

    public class CompleteAppTaskDto
    {
        [Required]
        public Guid AppTaskId { get; set; }

        [MaxLength(2000)]
        public string? ResultNotes { get; set; }
    }

    public class ToggleAppTaskChecklistDto
    {
        [Required]
        public Guid AppTaskId { get; set; }

        [Required]
        public Guid CheckListId { get; set; }

        public bool IsChecked { get; set; }
    }

    public class AppTaskListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? TaskDesciption { get; set; }

        public int? EntityTypeId { get; set; }
        public string? EntityTypeName { get; set; }
        public string? EntityReference { get; set; }
        public Guid? RelatedEntityId { get; set; }

        public int? TaskStatusId { get; set; }
        public string? TaskStatusName { get; set; }

        public int? PriorityId { get; set; }
        public string? PriorityName { get; set; }

        public Guid? OwnerUserId { get; set; }
        public string? OwnerUserName { get; set; }

        public Guid? CurrentAssignedToUserId { get; set; }
        public string? CurrentAssignedToUserName { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool IsCompleted => CompletedDate.HasValue;
        public bool IsOverDue => DueDate.HasValue && !CompletedDate.HasValue && DueDate.Value.Date < DateTime.Today;
    }

    public class AppTaskDetailsDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? TaskDesciption { get; set; }

        public int? EntityTypeId { get; set; }
        public string? EntityTypeName { get; set; }
        public Guid? RelatedEntityId { get; set; }

        public int? TaskStatusId { get; set; }
        public string? TaskStatusName { get; set; }

        public int? PriorityId { get; set; }
        public string? PriorityName { get; set; }

        public int? SourceTypeId { get; set; }
        public string? SourceTypeName { get; set; }

        public Guid? OwnerUserId { get; set; }
        public string? OwnerUserName { get; set; }

        public Guid? CurrentAssignedToUserId { get; set; }
        public string? CurrentAssignedToUserName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public bool CanComplete { get; set; }
        public bool CanComment { get; set; }
        public bool CanAssign { get; set; }
        public bool HasOpenMandatoryChecklist { get; set; }

        public List<AppTaskAssignmentDto> Assignments { get; set; } = new();
        public List<AppTaskCommentDto> Comments { get; set; } = new();
        public List<AppTaskChecklistItemDto> CheckLists { get; set; } = new();
        public List<AppTaskTimelineItemDto> Timeline { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
    }

    public class AppTaskAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid AppTaskId { get; set; }

        public Guid AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }

        public Guid AssignedByUserId { get; set; }
        public string? AssignedByUserName { get; set; }

        public DateTime AssignedDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Notes { get; set; }
    }

    public class AppTaskCommentDto
    {
        public Guid Id { get; set; }
        public Guid AppTaskId { get; set; }

        public string CommentText { get; set; } = string.Empty;
        public int CommentTypeId { get; set; }
        public bool IsInternal { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AppTaskChecklistItemDto
    {
        public Guid Id { get; set; }
        public Guid AppTaskId { get; set; }

        public string CheckListItemTitle { get; set; } = string.Empty;
        public string? Description { get; set; }

        public bool IsMandatory { get; set; }
        public bool IsChecked { get; set; }
        public int SortOrder { get; set; }

        public Guid? CheckedByUserId { get; set; }
        public string? CheckedByUserName { get; set; }
        public DateTime? CheckedDate { get; set; }
    }

    public class AppTaskTimelineItemDto
    {
        public string EventType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? UserName { get; set; }
        public int SortOrder { get; set; }
    }


    public class AppTaskIndexViewModel
    {
        public AppTaskSearchViewModel Search { get; set; } = new();
        public PagedResultDto<AppTaskListItemDto> Result { get; set; } = new();
    }

    public class AppTaskSearchViewModel
    {
        public AppTaskFilterDto Filter { get; set; } = new();

        public List<SelectListItem> TaskStatuses { get; set; } = new();
        public List<SelectListItem> Priorities { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> PageSizes { get; set; } = new();

        public bool ForceMyTasksOnly { get; set; }
        public bool CanChangeMyTasksOnly { get; set; } = true;

    }

    public class CreateAppTaskViewModel
    {
        public CreateAppTaskDto Input { get; set; } = new();

        public List<SelectListItem> Priorities { get; set; } = new();
        public List<SelectListItem> SourceTypes { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();

        public List<SelectListItem> Tasks { get; set; } = new();

    }

    public class AssignAppTaskViewModel
    {
        public AssignAppTaskDto Input { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
    }

    public class AppTaskDashboardItemDto
    {
        public Guid Id { get; set; }

        public string TaskTitle { get; set; } = "";
        public string? TaskDescription { get; set; }

        public int TaskStatusId { get; set; }
        public string TaskStatusName { get; set; } = "";

        public int? PriorityId { get; set; }
        public string? PriorityName { get; set; }

        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }

        public DateTime? DueDate { get; set; }

        public int? RelatedEntityTypeId { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string? RelatedEntityTitle { get; set; }
    }

    public class AppTaskDashboardWidgetViewModel
    {
        public string Filter { get; set; } = "my";
        public string? Keyword { get; set; }

        public List<AppTaskDashboardItemDto> Items { get; set; } = new();
    }

}
