using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.WebApp.ViewModels.TaskVms
{
    public class TaskCategoryViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string LocalCategoryName { get; set; }
        public bool IsActive { get; set; }
    }

    public class TaskViewModel
    {
        public int Id { get; set; }
        public int TaskCategoryId { get; set; }
        public string TaskName { get; set; }
        public string LocalTaskName { get; set; }
        public int? FollowUpPriorityId { get; set; }
        public string PriorityName { get; set; }
        public int? DueDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class TaskCheckListTemplateViewModel
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public string CheckListTitle { get; set; }
        public string Description { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class TaskManagementIndexViewModel
    {
        public List<TaskCategoryViewModel> Categories { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
        public List<TaskCheckListTemplateViewModel> CheckListTemplates { get; set; }

        public int? SelectedCategoryId { get; set; }
        public string SelectedCategoryName { get; set; }

        public int? SelectedTaskId { get; set; }
        public string SelectedTaskName { get; set; }

        public string SearchTaskName { get; set; }
    }
    public class TaskCreateEditViewModel
    {
        public int? Id { get; set; }
        public int TaskCategoryId { get; set; }
        public string TaskName { get; set; }
        public string LocalTaskName { get; set; }
        public int? FollowUpPriorityId { get; set; }
        public int? DueDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        public List<SelectListItem>? Priorities { get; set; }
    }

    public class CheckListCreateEditViewModel
    {
        public Guid? Id { get; set; }
        public int TaskId { get; set; }
        public string CheckListTitle { get; set; }
        public string Description { get; set; }
        public bool IsMandatory { get; set; }
    }

}
