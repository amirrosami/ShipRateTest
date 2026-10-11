using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Operations.TasksManage;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.TaskVms;
[Authorize]

public class TaskManagementController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaskManagementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TaskManagement/Index
    public async Task<IActionResult> Index()
    {
        var categories = await _context.TaskCategories
            .Where(c => c.IsActive)
            .Select(c => new TaskCategoryViewModel
            {
                Id = c.Id,
                CategoryName = c.TaskCategoryName,
                LocalCategoryName = c.TaskCategoryLocalName,
                IsActive = c.IsActive
            }).ToListAsync();

        var vm = new TaskManagementIndexViewModel
        {
            Categories = categories
        };

        return View(vm);
    }

    // HTMX: Load Tasks by Category
    public async Task<IActionResult> LoadTasks(int categoryId, string searchTaskName)
    {
        var tasksQuery = _context.Tasks
            .Include(t => t.DefaultPriority)
            .Where(t => t.TaskCategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(searchTaskName))
            tasksQuery = tasksQuery.Where(t => t.TaskName.Contains(searchTaskName));

        var tasks = await tasksQuery
            .Select(t => new TaskViewModel
            {
                Id = t.Id,
                TaskCategoryId = t.TaskCategoryId,
                TaskName = t.TaskName,
                LocalTaskName = t.TaskLocalName,
                FollowUpPriorityId = t.DefaultPriorityId,
                PriorityName = t.DefaultPriority.FollowUpPriorityName,
                DueDays = t.DefaultDueDays,
                Description = t.Description,
                IsActive = t.IsActive
            }).ToListAsync();

        ViewBag.CategoryId = categoryId;

        return PartialView("_TasksGrid", tasks);
    }

    // HTMX: Load Checklist by Task
    public async Task<IActionResult> LoadChecklist(int taskId)
    {
        var list = await _context.TaskCheckListTemplates
            .Where(c => c.TaskId == taskId)
            .Select(c => new TaskCheckListTemplateViewModel
            {
                Id = c.Id,
                TaskId = c.TaskId,
                CheckListTitle = c.CheckListItemTitle,
                Description = c.Description,
                IsMandatory = c.IsMandatory
            }).ToListAsync();

        ViewBag.TaskId = taskId;

        return PartialView("_ChecklistGrid", list);
    }

    // GET: Create Task (Modal)
    public IActionResult CreateTask(int categoryId)
    {
        var vm = new TaskCreateEditViewModel
        {
            TaskCategoryId = categoryId,
            Priorities = _context.FollowUpPriorities
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.FollowUpPriorityName
                }).ToList()
        };

        return PartialView("_TaskFormModal", vm);
    }

    // POST: Create Task
    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskCreateEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var Priorities = _context.FollowUpPriorities
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.FollowUpPriorityName
            }).ToList();
            vm.Priorities = Priorities;
            return PartialView("_TaskFormModal", vm);
        }

        var task = new ShipRate.Domain.Entities.Operations.TasksManage.Task
        {
            TaskCategoryId = vm.TaskCategoryId,
            TaskName = vm.TaskName,
            TaskLocalName = vm.LocalTaskName,
            DefaultPriorityId = vm.FollowUpPriorityId.Value,
            DefaultDueDays = vm.DueDays.Value,
            Description = vm.Description,
            IsActive = vm.IsActive
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return await LoadTasks(vm.TaskCategoryId, null);
    }

    // GET: Create Checklist
    public IActionResult CreateChecklist(int taskId)
    {
        var vm = new CheckListCreateEditViewModel
        {
            TaskId = taskId
        };

        return PartialView("_ChecklistFormModal", vm);
    }

    // POST: Create Checklist
    [HttpPost]
    public async Task<IActionResult> CreateChecklist(CheckListCreateEditViewModel vm)
    {
        if (!ModelState.IsValid)
            return PartialView("_ChecklistFormModal", vm);

        var entity = new TaskCheckListTemplate
        {
            TaskId = vm.TaskId,
            CheckListItemTitle = vm.CheckListTitle,
            Description = vm.Description,
            IsMandatory = vm.IsMandatory
        };

        _context.TaskCheckListTemplates.Add(entity);
        await _context.SaveChangesAsync();

        return await LoadChecklist(vm.TaskId);
    }



    [HttpGet]
    public async Task<IActionResult> EditTask(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.TaskCategory)
            .Include(t => t.DefaultPriority)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            return NotFound();

        var model = new TaskCreateEditViewModel
        {
            Id = task.Id,
            TaskCategoryId = task.TaskCategoryId,
            TaskName = task.TaskName,
            LocalTaskName = task.TaskLocalName,
            Description = task.Description,
            FollowUpPriorityId = task.DefaultPriorityId,
            IsActive = task.IsActive,
            Priorities = await _context.FollowUpPriorities
                .Where(p => p.IsActive)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.FollowUpPriorityName })
                .ToListAsync()
        };

        return PartialView("_TaskFormModal", model);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(TaskCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView("_TaskFormModal", model);

        var task = await _context.Tasks.FindAsync(model.Id);
        if (task == null)
            return NotFound();

        task.TaskCategoryId = model.TaskCategoryId;
        task.TaskName = model.TaskName;
        task.TaskLocalName = model.LocalTaskName;
        task.Description = model.Description;
        task.DefaultPriorityId = model.FollowUpPriorityId.Value;
        task.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        return await LoadTasks(model.TaskCategoryId, null);
    }

    [HttpGet]
    public async Task<IActionResult> EditChecklist(Guid checklistId)
    {
        var checklist = await _context.TaskCheckListTemplates
            .FirstOrDefaultAsync(c => c.Id == checklistId);

        if (checklist == null)
            return NotFound();

        var model = new CheckListCreateEditViewModel
        {
            Id = checklist.Id,
            TaskId = checklist.TaskId,
            CheckListTitle = checklist.CheckListItemTitle,
            Description = checklist.Description,
            IsMandatory = checklist.IsMandatory
        };

        return PartialView("_ChecklistFormModal", model);
    }

    [HttpPost]
    public async Task<IActionResult> EditChecklist(CheckListCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView("_ChecklistFormModal", model);

        var checklist = await _context.TaskCheckListTemplates.FindAsync(model.Id);
        if (checklist == null)
            return NotFound();

        checklist.CheckListItemTitle = model.CheckListTitle;
        checklist.Description = model.Description;
        checklist.IsMandatory = model.IsMandatory;
        //checklist.IsActive = true;
        checklist.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return await LoadChecklist(model.TaskId);
    }
}
