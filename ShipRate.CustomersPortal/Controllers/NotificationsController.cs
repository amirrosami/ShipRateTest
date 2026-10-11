using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Notif;
using ShipRate.Application.Interfaces.Notif;
using System.Security.Claims;

namespace ShipRate.WebAppCustomers.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private bool IsHtmxRequest()
        {
            return Request.Headers.ContainsKey("HX-Request");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Manage(
            [FromQuery] NotificationAdminFilterDto filter,
            CancellationToken ct)
        {
            var model = await _notificationService.GetAdminListAsync(filter, ct);

            if (IsHtmxRequest())
            {
                return PartialView("_AdminList", model);
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var model = await _notificationService.GetDetailsAsync(id, ct);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            [FromQuery] NotificationListFilterDto filter,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var model = await _notificationService.GetUserNotificationsAsync(
                currentUserId,
                filter,
                cancellationToken);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = await _notificationService.GetCreateModelAsync(cancellationToken);

            return PartialView("_CreateModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateManualNotificationRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var result = await _notificationService.CreateManualAsync(
                request,
                currentUserId,
                cancellationToken);

            if (!result.Succeeded)
            {
                Response.StatusCode = 400;

                var model = await _notificationService.GetCreateModelAsync(cancellationToken);

                model.Title = request.Title;
                model.Message = request.Message;
                model.NotificationTypeId = request.NotificationTypeId;
                model.SeverityId = request.SeverityId;
                model.PriorityId = request.PriorityId;
                model.ReferenceNo = request.ReferenceNo;
                model.TargetUrl = request.TargetUrl;
                model.IconCssClass = request.IconCssClass;
                model.PublishAt = request.PublishAt;
                model.ExpireAt = request.ExpireAt;
                model.RequiresAction = request.RequiresAction;
                model.SelectedUserTypeId = request.SelectedUserTypeId;
                model.SelectedUserIds = request.SelectedUserIds ?? new List<Guid>();

                model.Users = await _notificationService.GetUsersByTypeAsync(
                    request.SelectedUserTypeId,
                    cancellationToken);

                ViewBag.ErrorMessage = result.Message;

                return PartialView("_CreateModal", model);
            }

            Response.Headers["HX-Trigger"] = "notificationCreated";

            return Content("");
        }

        [HttpGet]
        public async Task<IActionResult> UsersByType(
            int selectedUserTypeId,
            CancellationToken cancellationToken)
        {
            var users = await _notificationService.GetUsersByTypeAsync(
                selectedUserTypeId,
                cancellationToken);

            return PartialView("_UserMultiSelect", users);
        }

        [HttpGet]
        public async Task<IActionResult> Navbar(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var model = await _notificationService.GetNavbarNotificationsAsync(
                currentUserId,
                5,
                cancellationToken);

            return PartialView("_NavbarNotifications", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(
            Guid userNotificationId,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var result = await _notificationService.MarkAsReadAsync(
                userNotificationId,
                currentUserId,
                cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            var result = await _notificationService.MarkAllAsReadAsync(
                currentUserId,
                cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Archive(
            Guid id,
            NotificationAdminFilterDto filter,
            CancellationToken ct)
        {
            var currentUserId = GetCurrentUserId();

            var result = await _notificationService.ArchiveAsync(id, currentUserId, ct);

            if (!result.Succeeded)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Content(result.Message);
            }

            var model = await _notificationService.GetAdminListAsync(filter, ct);

            Response.Headers["HX-Trigger"] = "notificationChanged";

            return PartialView("_AdminList", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Duplicate(Guid id, CancellationToken ct)
        {
            var model = await _notificationService.BuildDuplicateAsync(id, ct);

            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_CreateModal", model);
        }




        private Guid GetCurrentUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            return Guid.Parse(value);
        }
    }
}
