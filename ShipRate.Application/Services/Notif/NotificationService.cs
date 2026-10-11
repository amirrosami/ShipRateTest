using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Notif;
using ShipRate.Application.Hubs;
using ShipRate.Application.Interfaces.Notif;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.Notifs;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Notif
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly INotificationRuleService _notificationRuleService;
        private readonly IHubContext<NotificationHub> _notificationHub;

        private const string ManualSourceTypeName = "Manual";
        public NotificationService(ApplicationDbContext dbContext, INotificationRuleService notificationRuleService, IHubContext<NotificationHub> notificationHub)
        {
            _db = dbContext;
            _notificationRuleService = notificationRuleService;
            _notificationHub = notificationHub;
        }

        public async Task<CreateNotificationDto> GetCreateModelAsync(
            CancellationToken cancellationToken = default)
        {
            var model = new CreateNotificationDto
            {
                PublishAt = DateTime.Now
            };

            model.NotificationTypes = await _db.EventTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.EventTypeName)
                .Select(x => new SelectListItemDto
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeName
                })
                .ToListAsync(cancellationToken);

            model.Severities = await _db.NotificationSeverities
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItemDto
                {
                    Value = x.Id.ToString(),
                    Text = x.SeverityName
                })
                .ToListAsync(cancellationToken);

            model.Priorities = await _db.FollowUpPriorities
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItemDto
                {
                    Value = x.Id.ToString(),
                    Text = x.FollowUpPriorityName
                })
                .ToListAsync(cancellationToken);

            model.UserTypes = await _db.UserTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.UserTypeName)
                .Select(x => new SelectListItemDto
                {
                    Value = x.Id.ToString(),
                    Text = x.UserTypeName
                })
                .ToListAsync(cancellationToken);

            return model;
        }

        public async Task<List<SelectListItemDto>> GetUsersByTypeAsync(
            int selectedUserTypeId,
            CancellationToken cancellationToken = default)
        {
            if (selectedUserTypeId <= 0)
            {
                return new List<SelectListItemDto>();
            }

            var users = await _db.Users
                .Where(x =>
                    x.IsActive &&
                    x.UserTypeId == selectedUserTypeId)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItemDto
                {
                    Value = x.Id.ToString(),
                    Text =
                        x.FullName != null && x.FullName != ""
                            ? x.FullName
                            : x.UserName != null && x.UserName != ""
                                ? x.UserName
                                : x.Email != null && x.Email != ""
                                    ? x.Email
                                    : x.Id.ToString()
                })
                .ToListAsync(cancellationToken);

            return users;
        }

        public async Task<ServiceResultDto<Guid>> CreateManualAsync(
            CreateManualNotificationRequest request,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await ValidateCreateManualAsync(
                request,
                cancellationToken);

            if (!validationResult.Succeeded)
            {
                return ServiceResultDto<Guid>.Fail(validationResult.Message ?? "Invalid request.");
            }

            var distinctUserIds = request.SelectedUserIds
                .Distinct()
                .ToList();

            var validUserIds = await _db.Users
                .Where(x =>
                    x.IsActive &&
                    x.UserTypeId == request.SelectedUserTypeId &&
                    distinctUserIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var manualSourceTypeId = await _db.NotificationSourceTypes
                .Where(x =>
                    x.IsActive &&
                    !x.IsDeleted &&
                    x.SourceTypeName == ManualSourceTypeName)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (manualSourceTypeId <= 0)
            {
                return ServiceResultDto<Guid>.Fail("Manual source type is not configured.");
            }

            var now = DateTime.Now;

            var notification = new Notification
            {
                SourceTypeId = manualSourceTypeId,
                NotificationTypeId = request.NotificationTypeId,
                SeverityId = request.SeverityId,
                PriorityId = request.PriorityId,

                Title = request.Title.Trim(),
                Message = string.IsNullOrWhiteSpace(request.Message)
                    ? null
                    : request.Message.Trim(),

                ReferenceNo = string.IsNullOrWhiteSpace(request.ReferenceNo)
                    ? null
                    : request.ReferenceNo.Trim(),

                TargetUrl = string.IsNullOrWhiteSpace(request.TargetUrl)
                    ? null
                    : request.TargetUrl.Trim(),

                IconCssClass = string.IsNullOrWhiteSpace(request.IconCssClass)
                    ? null
                    : request.IconCssClass.Trim(),

                ActivityLogId = null,
                EntityTypeId = null,
                EntityId = null,

                PublishAt = request.PublishAt,
                ExpireAt = request.ExpireAt,

                IsBroadcast = false,
                RequiresAction = request.RequiresAction,

                //IsActive = true,
                IsDeleted = false,

                CreatedAt = now,
                UpdatedAt = now,
                CreateUserId = currentUserId,
                EditUserId = currentUserId,
                IsArchived = false
            };

            await _db.AppNotifications.AddAsync(notification, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            var userNotifications = validUserIds
                .Select(userId => new UserNotification
                {
                    NotificationId = notification.Id,
                    UserId = userId,

                    IsRead = false,
                    ReadAt = null,

                    IsSeen = false,
                    SeenAt = null,

                    IsHidden = false,
                    HiddenAt = null,

                    IsPinned = false,

                    //IsActive = true,
                    IsDeleted = false,

                    CreatedAt = now,
                    UpdatedAt = now,
                    CreateUserId = currentUserId,
                    EditUserId = currentUserId
                })
                .ToList();

            await _db.UserNotifications.AddRangeAsync(userNotifications, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);




            foreach (var userId in userNotifications)
            {
                await _notificationHub.Clients
                    .Group($"user-{userId.UserId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.Id,
                        title = notification.Title,
                        message = notification.Message,
                        createdAt = notification.CreatedAt
                    });
            }




            return ServiceResultDto<Guid>.Success(
                notification.Id,
                "Notification created successfully.");
        }

        public async Task<NavbarNotificationsViewModel> GetNavbarNotificationsAsync(
            Guid userId,
            int take = 5,
            CancellationToken cancellationToken = default)
        {
            if (take <= 0)
            {
                take = 5;
            }

            var now = DateTime.Now;

            var query = _db.UserNotifications
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted &&
                    !x.Notification.IsDeleted &&
                    x.Notification.PublishAt <= now &&
                    (
                        x.Notification.ExpireAt == null ||
                        x.Notification.ExpireAt >= now
                    ));

            var unreadCount = await query
                .CountAsync(x => !x.IsRead, cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Notification.CreatedAt)
                .Take(take)
                .Select(x => new NavbarNotificationDto
                {
                    UserNotificationId = x.Id,
                    NotificationId = x.NotificationId,
                    Title = x.Notification.Title,
                    Message = x.Notification.Message,
                    TargetUrl = x.Notification.TargetUrl,
                    IconCssClass = x.Notification.IconCssClass,
                    SeverityId = x.Notification.SeverityId,
                    IsRead = x.IsRead,
                    CreatedAt = x.Notification.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new NavbarNotificationsViewModel
            {
                UnreadCount = unreadCount,
                Items = items
            };
        }

        public async Task<PagedResultDto<NotificationListItemDto>> GetUserNotificationsAsync(
            Guid userId,
            NotificationListFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            if (filter.PageNumber <= 0)
            {
                filter.PageNumber = 1;
            }

            if (filter.PageSize <= 0)
            {
                filter.PageSize = 10;
            }

            if (filter.PageSize > 50)
            {
                filter.PageSize = 50;
            }

            var query = _db.UserNotifications
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted &&
                    !x.Notification.IsDeleted);

            if (filter.IsRead.HasValue)
            {
                query = query.Where(x => x.IsRead == filter.IsRead.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.Trim();

                query = query.Where(x =>
                    x.Notification.Title.Contains(searchTerm) ||
                    (
                        x.Notification.Message != null &&
                        x.Notification.Message.Contains(searchTerm)
                    ) ||
                    (
                        x.Notification.ReferenceNo != null &&
                        x.Notification.ReferenceNo.Contains(searchTerm)
                    ));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.IsPinned)
                .ThenByDescending(x => x.Notification.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new NotificationListItemDto
                {
                    UserNotificationId = x.Id,
                    NotificationId = x.NotificationId,

                    Title = x.Notification.Title,
                    Message = x.Notification.Message,
                    ReferenceNo = x.Notification.ReferenceNo,
                    TargetUrl = x.Notification.TargetUrl,
                    IconCssClass = x.Notification.IconCssClass,

                    SeverityId = x.Notification.SeverityId,

                    IsRead = x.IsRead,
                    IsSeen = x.IsSeen,
                    IsPinned = x.IsPinned,

                    CreatedAt = x.Notification.CreatedAt,
                    ReadAt = x.ReadAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<NotificationListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ServiceResultDto> MarkAsReadAsync(
            Guid userNotificationId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var userNotification = await _db.UserNotifications
                .FirstOrDefaultAsync(x =>
                    x.Id == userNotificationId &&
                    x.UserId == userId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (userNotification == null)
            {
                return ServiceResultDto.Fail("Notification not found.");
            }

            if (!userNotification.IsRead)
            {
                var now = DateTime.Now;

                userNotification.IsRead = true;
                userNotification.ReadAt = now;

                userNotification.IsSeen = true;
                userNotification.SeenAt = now;

                userNotification.UpdatedAt = now;
                userNotification.EditUserId = userId;

                await _db.SaveChangesAsync(cancellationToken);
            }

            return ServiceResultDto.Success();
        }

        public async Task<ServiceResultDto> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var items = await _db.UserNotifications
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsDeleted &&
                    !x.IsRead)
                .ToListAsync(cancellationToken);

            var now = DateTime.Now;

            foreach (var item in items)
            {
                item.IsRead = true;
                item.ReadAt = now;

                item.IsSeen = true;
                item.SeenAt = now;

                item.UpdatedAt = now;
                item.EditUserId = userId;
            }

            await _db.SaveChangesAsync(cancellationToken);

            return ServiceResultDto.Success();
        }

        private async Task<ServiceResultDto> ValidateCreateManualAsync(
            CreateManualNotificationRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return ServiceResultDto.Fail("Request is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return ServiceResultDto.Fail("Title is required.");
            }

            if (request.Title.Length > 250)
            {
                return ServiceResultDto.Fail("Title is too long.");
            }

            if (request.NotificationTypeId <= 0)
            {
                return ServiceResultDto.Fail("Notification type is required.");
            }

            if (request.SeverityId <= 0)
            {
                return ServiceResultDto.Fail("Severity is required.");
            }

            if (request.PriorityId <= 0)
            {
                return ServiceResultDto.Fail("Priority is required.");
            }

            if (request.SelectedUserTypeId <= 0)
            {
                return ServiceResultDto.Fail("User type is required.");
            }

            if (request.SelectedUserIds == null || request.SelectedUserIds.Count == 0)
            {
                return ServiceResultDto.Fail("At least one user must be selected.");
            }

            if (request.ExpireAt.HasValue && request.ExpireAt.Value < request.PublishAt)
            {
                return ServiceResultDto.Fail("Expire date must be greater than publish date.");
            }

            var notificationTypeExists = await _db.EventTypes
                .AnyAsync(x =>
                    x.Id == request.NotificationTypeId &&
                    x.IsActive &&
                    !x.IsDeleted,
                    cancellationToken);

            if (!notificationTypeExists)
            {
                return ServiceResultDto.Fail("Invalid notification type.");
            }

            var severityExists = await _db.NotificationSeverities
                .AnyAsync(x =>
                    x.Id == request.SeverityId &&
                    x.IsActive &&
                    !x.IsDeleted,
                    cancellationToken);

            if (!severityExists)
            {
                return ServiceResultDto.Fail("Invalid severity.");
            }

            var priorityExists = await _db.FollowUpPriorities
                .AnyAsync(x =>
                    x.Id == request.PriorityId &&
                    x.IsActive &&
                    !x.IsDeleted,
                    cancellationToken);

            if (!priorityExists)
            {
                return ServiceResultDto.Fail("Invalid priority.");
            }

            var userTypeExists = await _db.UserTypes
                .AnyAsync(x =>
                    x.Id == request.SelectedUserTypeId &&
                    x.IsActive &&
                    !x.IsDeleted,
                    cancellationToken);

            if (!userTypeExists)
            {
                return ServiceResultDto.Fail("Invalid user type.");
            }

            var distinctUserIds = request.SelectedUserIds
                .Distinct()
                .ToList();

            var validUsersCount = await _db.Users
                .CountAsync(x =>
                    x.IsActive &&
                    x.UserTypeId == request.SelectedUserTypeId &&
                    distinctUserIds.Contains(x.Id),
                    cancellationToken);

            if (validUsersCount != distinctUserIds.Count)
            {
                return ServiceResultDto.Fail("One or more selected users are invalid.");
            }
            return ServiceResultDto.Success();
        }




        public async Task<NotificationAdminListViewModel> GetAdminListAsync(
            NotificationAdminFilterDto filter,
            CancellationToken ct = default)
        {
            if (filter.Page < 1)
                filter.Page = 1;

            if (filter.PageSize < 1)
                filter.PageSize = 20;

            var query = _db.AppNotifications
                .Include(x => x.UserNotifications)
                .Include(x => x.CreatedByUser)
                .Include(x => x.SourceType)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Message.Contains(search));
            }

            if (filter.SourceTypeId.HasValue)
            {
                query = query.Where(x => x.SourceTypeId == filter.SourceTypeId.Value);
            }

            if (filter.IsArchived.HasValue)
            {
                query = query.Where(x => x.IsArchived == filter.IsArchived.Value);
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedAt < toDate);
            }

            if (filter.UserTypeId.HasValue)
            {
                query = query.Where(x =>
                    x.UserNotifications.Any(un =>
                        un.User.UserTypeId == filter.UserTypeId.Value));
            }

            if (filter.HasUnread == true)
            {
                query = query.Where(x =>
                    x.UserNotifications.Any(un => !un.IsRead));
            }

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new NotificationAdminListItemDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    MessagePreview = x.Message.Length > 120
                        ? x.Message.Substring(0, 120) + "..."
                        : x.Message,
                    SourceTypeTitle = x.SourceType.SourceTypeName,
                    CreatedByName = x.CreatedByUser.FullName,
                    CreatedAt = x.CreatedAt,
                    TotalRecipients = x.UserNotifications.Count,
                    ReadCount = x.UserNotifications.Count(un => un.IsRead),
                    IsArchived = x.IsArchived.Value,
                    ArchivedAt = x.ArchivedAt
                })
                .ToListAsync(ct);

            return new NotificationAdminListViewModel
            {
                Filter = filter,
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<NotificationDetailsDto?> GetDetailsAsync(
            Guid notificationId,
            CancellationToken ct = default)
        {
            return await _db.AppNotifications
                .AsNoTracking()
                .Where(x => x.Id == notificationId)
                .Select(x => new NotificationDetailsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    TargetUrl = x.TargetUrl,
                    IconCssClass = x.IconCssClass,
                    SourceTypeTitle = x.SourceType.SourceTypeName,
                    CreatedByName = x.CreatedByUser.FullName,
                    CreatedAt = x.CreatedAt,
                    TotalRecipients = x.UserNotifications.Count,
                    ReadCount = x.UserNotifications.Count(un => un.IsRead),
                    IsArchived = x.IsArchived.Value,
                    ArchivedAt = x.ArchivedAt,
                    ArchivedByName = x.ArchivedByUser != null
                        ? x.ArchivedByUser.FullName
                        : null,

                    Recipients = x.UserNotifications
                        .OrderBy(un => un.User.FullName)
                        .Select(un => new NotificationRecipientDto
                        {
                            UserId = un.UserId,
                            FullName = un.User.FullName,
                            UserName = un.User.UserName,
                            UserTypeTitle = un.User.UserType.UserTypeName,
                            IsRead = un.IsRead,
                            ReadAt = un.ReadAt,
                            CreatedAt = un.CreatedAt
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);
        }



        public async Task<ServiceResultDto> ArchiveAsync(
            Guid notificationId,
            Guid currentUserId,
            CancellationToken ct = default)
        {
            var notification = await _db.AppNotifications
                .FirstOrDefaultAsync(x => x.Id == notificationId, ct);

            if (notification == null)
            {
                return ServiceResultDto.Fail("Notification not found.");
            }

            if (notification.IsArchived.Value)
            {
                return ServiceResultDto.Fail("Notification is already archived.");
            }

            notification.IsArchived = true;
            notification.ArchivedAt = DateTime.Now;
            notification.ArchivedByUserId = currentUserId;

            await _db.SaveChangesAsync(ct);

            return ServiceResultDto.Success("Notification archived successfully.");
        }


        public async Task<CreateManualNotificationRequest?> BuildDuplicateAsync(
            Guid notificationId,
            CancellationToken ct = default)
        {
            var notification = await _db.AppNotifications
                .AsNoTracking()
                .Where(x => x.Id == notificationId)
                .Select(x => new CreateManualNotificationRequest
                {
                    Title = x.Title,
                    Message = x.Message,
                    TargetUrl = x.TargetUrl,
                    IconCssClass = x.IconCssClass
                })
                .FirstOrDefaultAsync(ct);

            return notification;
        }


        public async Task CreateForActivityLogAsync(Guid activityLogId, CancellationToken ct = default)
        {
            var activityLog = await _db.ActivityLogs
                .Include(x => x.EntityType)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == activityLogId, ct);

            if (activityLog == null)
                return;

            var rule = await _notificationRuleService.GetRuleForActivityAsync(
                activityLog.ActivityType,
                activityLog.EntityTypeId.Value,
                ct);

            //if (rule == null || !rule.ShouldNotify)
            //    return;

            //var recipientUserIds = await _notificationRecipientResolver.ResolveAsync(
            //    activityLog,
            //    rule,
            //    ct);


            var recipientUserIds = new List<Guid> { };

            if (!recipientUserIds.Any())
                return;

            var notification = new Notification
            {
                Title = BuildText(rule.TitleTemplate, activityLog) ?? "",
                Message = BuildText(rule.MessageTemplate, activityLog) ?? "",
                TargetUrl = BuildText(rule.TargetUrlTemplate, activityLog),
                SourceTypeId = 6,
                CreateUserId = activityLog.CreateUserId,
                CreatedAt = DateTime.Now,
                UserNotifications = recipientUserIds
                    .Distinct()
                    .Select(userId => new UserNotification
                    {
                        UserId = userId,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    })
                    .ToList()
            };

            _db.AppNotifications.Add(notification);
            await _db.SaveChangesAsync(ct);
        }
        private static string? BuildText(string? template, ActivityLog activityLog)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return template;
            }
            return template
                .Replace("{ActivityLogId}", activityLog.Id.ToString())
                .Replace("{ActivityType}", activityLog.ActivityType ?? "")
                .Replace("{EntityType}", activityLog.EntityType.EntityTypeName ?? "")
                .Replace("{EntityId}", activityLog.EntityId.ToString() ?? "");
        }
    }
}
