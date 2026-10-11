//به checkbox اعتماد نکن؛ فیلتر Mine اجباری را در Backend enforce کن.
//به checkbox اعتماد نکن؛ فیلتر Mine اجباری را در Backend enforce کن.
//به checkbox اعتماد نکن؛ فیلتر Mine اجباری را در Backend enforce کن.

using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.AppTasks;
using ShipRate.Application.Interfaces;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    public class AppTasksController : Controller
    {
        private readonly IAppTaskService _appTaskService;
        private readonly ICurrentUserService _currentUserService;

        public AppTasksController(IAppTaskService appTaskService, ICurrentUserService currentUserService)
        {
            _appTaskService = appTaskService;
            _currentUserService = currentUserService;

        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }



        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AppTaskFilterDto filter, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var forceMyTasksOnly = ShouldForceMyTasksOnly();

            if (forceMyTasksOnly)
            {
                filter.MyTasksOnly = true;
            }



            var model = await _appTaskService.GetIndexDataAsync(
                filter ?? new AppTaskFilterDto(),
                currentUserId,
                cancellationToken);

            model.Search.ForceMyTasksOnly = forceMyTasksOnly;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AppTaskFilterDto filter, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var forceMyTasksOnly = ShouldForceMyTasksOnly();
            if (forceMyTasksOnly)
            {
                filter.MyTasksOnly = true;
            }




            var model = await _appTaskService.SearchAsync(
                filter ?? new AppTaskFilterDto(),
                currentUserId,
                cancellationToken);

            return PartialView("_TaskList", model);
        }
        [HttpGet]
        public async Task<IActionResult> Create(
            int? taskId = null,
            int? entityTypeId = null,
            Guid? relatedEntityId = null,
            CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();

            var model = await _appTaskService.GetCreateDataAsync(
                taskId,
                entityTypeId,
                relatedEntityId,
                currentUserId,
                cancellationToken);



            return PartialView("_CreateModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateAppTaskViewModel model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var currentUserId = GetCurrentUserId();

                var reloadModel = await _appTaskService.GetCreateDataAsync(
                    model.Input.TaskId,
                    model.Input.EntityTypeId,
                    model.Input.RelatedEntityId,
                    currentUserId,
                    cancellationToken);

                reloadModel.Input = model.Input;
                return PartialView("_CreateModal", reloadModel);
            }

            var userId = GetCurrentUserId();

            await _appTaskService.CreateAsync(
                model.Input,
                userId,
                cancellationToken);


            if (model.Input.RelatedEntityId != null)
            {
                HxTrigger(new
                {
                    closeModal = new { },
                    toastSuccess = "Task created sussefully!!!!"
                });
                return Content("");
            }

            Response.Headers["HX-Trigger"] = "appTaskListChanged,closeAppTaskModal";
            return Content(string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var model = await _appTaskService.GetDetailsAsync(
                id,
                currentUserId,
                cancellationToken);

            return PartialView("~/Views/AppTasks/_DetailsDrawer.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(
            AssignAppTaskDto input,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentUserId = GetCurrentUserId();

            await _appTaskService.AssignAsync(
                input,
                currentUserId,
                cancellationToken);

            var model = await _appTaskService.GetDetailsAsync(
                input.AppTaskId,
                currentUserId,
                cancellationToken);

            Response.Headers["HX-Trigger"] = "appTaskListChanged";
            return PartialView("_DetailsDrawer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(
            AddAppTaskCommentDto input,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentUserId = GetCurrentUserId();

            await _appTaskService.AddCommentAsync(
                input,
                currentUserId,
                cancellationToken);

            var model = await _appTaskService.GetDetailsAsync(
                input.AppTaskId,
                currentUserId,
                cancellationToken);

            return PartialView("_DetailsDrawer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(
            CompleteAppTaskDto input,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentUserId = GetCurrentUserId();

            await _appTaskService.CompleteAsync(
                input,
                currentUserId,
                cancellationToken);

            var model = await _appTaskService.GetDetailsAsync(
                input.AppTaskId,
                currentUserId,
                cancellationToken);

            Response.Headers["HX-Trigger"] = "appTaskListChanged";
            return PartialView("_DetailsDrawer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleChecklist(
            ToggleAppTaskChecklistDto input,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var currentUserId = GetCurrentUserId();

            await _appTaskService.ToggleCheckListItemAsync(
                input,
                currentUserId,
                cancellationToken);

            var model = await _appTaskService.GetDetailsAsync(
                input.AppTaskId,
                currentUserId,
                cancellationToken);

            return PartialView("_DetailsDrawer", model);
        }

        private Guid GetCurrentUserId()
        {
            return _currentUserService.UserId;
        }
        [HttpPost]
        public async Task<IActionResult> Start(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            await _appTaskService.StartAsync(id, currentUserId);
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new
            {
                showToast = new
                {
                    message = "Task started.",
                    type = "success"
                },
                appTaskChanged = new { }
            });
            var model = await _appTaskService.GetDetailsAsync(
                id,
                currentUserId);

            return PartialView("_DetailsDrawer", model);
        }


        private bool ShouldForceMyTasksOnly()
        {
            // نمونه:
            // کاربران عادی فقط Taskهای خودشان را ببینند
            // مدیرها، ادمین‌ها یا Supervisorها بتوانند همه را ببینند

            //return !User.IsInRole("Admin")
            //    && !User.IsInRole("Manager")
            //    && !User.IsInRole("Supervisor");
            return true;
        }


        [HttpGet]
        public async Task<IActionResult> DashboardWidget(
            string filter = "my",
            string? keyword = null)
        {
            var currentUserId = GetCurrentUserId();

            var items = await _appTaskService.GetDashboardTasksAsync(
                currentUserId,
                filter,
                keyword,
                take: 5);

            var model = new AppTaskDashboardWidgetViewModel
            {
                Filter = filter,
                Keyword = keyword,
                Items = items
            };

            return PartialView("_DashboardTasksWidget", model);
        }



    }
}
