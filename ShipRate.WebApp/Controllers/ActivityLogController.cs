using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class ActivityLogController : Controller
    {
        private readonly IActivityLogService _activityLogService;
        private readonly ICurrentUserService _currentUserService;

        public ActivityLogController(IActivityLogService activityLogService, ICurrentUserService currentUserService)
        {
            _activityLogService = activityLogService;
            _currentUserService = currentUserService;
        }

        // باز کردن مدال اصلی شامل ساختار کلی و نگهدارنده لیست
        [HttpGet]
        public IActionResult IndexModal(int entityTypeId, Guid entityId, string? externalReferenceNo)
        {
            var vm = new ActivityLogWrapperVm
            {
                EntityTypeId = entityTypeId,
                EntityId = entityId,
                ExternalReferenceNo = externalReferenceNo
            };
            return PartialView("_ActivityLogModal", vm);
        }

        // لود گرید داده‌ها به صورت HTMX
        [HttpGet]
        public async Task<IActionResult> Grid(int entityTypeId, Guid entityId, CancellationToken cancellationToken)
        {
            var items = await _activityLogService.GetEntityTimelineAsync(entityTypeId, entityId, cancellationToken);
            var vm = new ActivityLogGridVm
            {
                EntityTypeId = entityTypeId,
                EntityId = entityId,
                Items = items
            };
            return PartialView("_ActivityLogGrid", vm);
        }
        // فرم افزودن رویداد جدید
        [HttpGet]
        public async Task<IActionResult> Create(int entityTypeId, Guid entityId, string? externalReferenceNo, CancellationToken cancellationToken)
        {
            var eventTypes = await _activityLogService.GetEventTypesLookupAsync(entityTypeId, cancellationToken);
            var vm = new ActivityLogUpsertFormVm
            {
                EntityTypeId = entityTypeId,
                EntityId = entityId,
                ActivityDateTime = DateTime.Now,
                ExternalReferenceNo = externalReferenceNo,
                EventTypes = eventTypes.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeName
                }).ToList()
            };

            return PartialView("_CreateEditModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityLogUpsertFormVm vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var eventTypes = await _activityLogService.GetEventTypesLookupAsync(vm.EntityTypeId, cancellationToken);
                vm.EventTypes = eventTypes.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeName
                }).ToList();

                return PartialView("_CreateEditModal", vm);
            }
            var request = new CreateActivityLogRequest
            {
                EntityTypeId = vm.EntityTypeId,
                EntityId = vm.EntityId,
                EventTypeId = vm.EventTypeId,
                Title = vm.Title,
                Description = vm.Description,
                ActivityDateTime = vm.ActivityDateTime,
                VisibleForCustomers = vm.VisibleForCustomers,
                IsSystemGenerated = false,
                ActivityCategoryId = 1,   //Event
                PerformedByUserId = _currentUserService.UserId,
                ExternalReferenceNo = vm.ExternalReferenceNo,
                InternalNote = vm.InternalNote
            };
            await _activityLogService.CreateAsync(request, cancellationToken);
            // ارسال سیگنال HTMX جهت بروزرسانی گرید اصلی
            Response.Headers.Append("HX-Trigger", "activityLogChanged");
            return Content(string.Empty);
        }

        // فرم ویرایش رویداد
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var details = await _activityLogService.GetByIdAsync(id, cancellationToken);
            if (details == null) return NotFound();

            var eventTypes = await _activityLogService.GetEventTypesLookupAsync(details.EntityTypeId, cancellationToken);
            var vm = new ActivityLogUpsertFormVm
            {
                Id = details.Id,
                EntityTypeId = details.EntityTypeId,
                EntityId = details.EntityId,
                EventTypeId = details.EventTypeId,
                Title = details.Title,
                Description = details.Description,
                ActivityDateTime = details.ActivityDateTime,
                VisibleForCustomers = details.VisibleForCustomers,
                ExternalReferenceNo = details.ExternalReferenceNo,
                InternalNote = details.InternalNote,
                EventTypes = eventTypes.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeName
                }).ToList()
            };

            return PartialView("_CreateEditModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityLogUpsertFormVm vm, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var eventTypes = await _activityLogService.GetEventTypesLookupAsync(vm.EntityTypeId, cancellationToken);
                vm.EventTypes = eventTypes.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeName
                }).ToList();

                return PartialView("_CreateEditModal", vm);
            }

            var request = new UpdateActivityLogRequest
            {
                Id = vm.Id!.Value,
                EventTypeId = vm.EventTypeId,
                Title = vm.Title,
                Description = vm.Description,
                ActivityDateTime = vm.ActivityDateTime,
                VisibleForCustomers = vm.VisibleForCustomers,
                ExternalReferenceNo = vm.ExternalReferenceNo,
                InternalNote = vm.InternalNote
            };

            await _activityLogService.UpdateAsync(request, cancellationToken);

            Response.Headers.Append("HX-Trigger", "activityLogChanged");
            return Content(string.Empty);
        }

        // حذف رویداد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _activityLogService.DeleteAsync(id, cancellationToken);
            Response.Headers.Append("HX-Trigger", "activityLogChanged");
            return Content(string.Empty);
        }
    }
}