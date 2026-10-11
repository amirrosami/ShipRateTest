using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Notif;
using ShipRate.Application.ViewModels;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Infrastructure.Data;
namespace ShipRate.Application.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;


        public ActivityLogService(ApplicationDbContext context, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<Guid> CreateAsync(
            CreateActivityLogRequest request,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;

            var log = new ActivityLog
            {
                Id = Guid.CreateVersion7(),

                EntityTypeId = request.EntityTypeId,
                EntityId = request.EntityId,

                ActivityType = request.ActivityType,
                ActivityCategoryId = request.ActivityCategoryId,

                EventTypeId = request.EventTypeId,

                Title = request.Title,
                Description = request.Description,
                ReferenceNo = request.ReferenceNo,

                ActivityDateTime = request.ActivityDateTime ?? now,
                CreatedAt = now,

                FromStatusTable = request.FromStatusEntityType,
                FromStatusId = request.FromStatusId,

                ToStatusTable = request.ToStatusEntityType,
                ToStatusId = request.ToStatusId,

                FieldName = request.FieldName,
                OldValue = request.OldValue,
                NewValue = request.NewValue,
                ChangeSetJson = request.ChangeSetJson,


                LocationId = request.LocationId,

                PerformedByUserId = request.PerformedByUserId,

                IsSystemGenerated = request.IsSystemGenerated,

                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,

                Severity = request.Severity,
                IconCssClass = request.IconCssClass,

                InternalNote = request.InternalNote,
                SourceSystem = request.SourceSystem,
                ExternalReferenceNo = request.ExternalReferenceNo,
                CreateUserId = _currentUserService.UserId,
                VisibleForCustomers = request.VisibleForCustomers,
                IsDeleted = false
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);
            await _notificationService.CreateForActivityLogAsync(log.Id, cancellationToken);
            return log.Id;
        }

        public Task LogEventAsync(
            CreateActivityLogRequest request,
            CancellationToken cancellationToken = default)
        {
            request.ActivityCategoryId = 1;
            return CreateAsync(request, cancellationToken);
        }

        public Task LogStatusChangeAsync(
            CreateActivityLogRequest request,
            CancellationToken cancellationToken = default)
        {
            request.ActivityCategoryId = 3;
            request.ActivityType = ActivityTypes.StatusChanged;
            return CreateAsync(request, cancellationToken);
        }

        public Task LogHistoryAsync(
            CreateActivityLogRequest request,
            CancellationToken cancellationToken = default)
        {
            request.ActivityCategoryId = 2;
            return CreateAsync(request, cancellationToken);
        }

        public Task LogAuditAsync(
            CreateActivityLogRequest request,
            CancellationToken cancellationToken = default)
        {
            request.ActivityCategoryId = 4;
            return CreateAsync(request, cancellationToken);
        }

        public async Task<List<ActivityLogListItemVm>> GetEntityTimelineAsync(
            int entityTypeId,
            Guid entityId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ActivityLogs
                .Include(x => x.EntityType)
                .Include(x => x.ActivityCategory)
                .Include(x => x.Location)
                .Include(x => x.PerformedByUser)
                .AsNoTracking()
                .AsQueryable();


            if (entityTypeId == 1)
            {

                var bls = _context.BLs
                    .Where(x => x.JobId == entityId)
                    .Select(x => x.Id);

                var bookings = _context.Bookings
                    .Where(x => x.JobId == entityId)
                    .Select(x => x.Id);

                var invoices = _context.Invoices
                    .Where(x => x.JobId == entityId)
                    .Select(x => x.Id);

                var charges = _context.Charges
                    .Where(x => x.JobId == entityId)
                    .Select(x => x.Id);

                var shipmentLegs = _context.ShipmentLegs
                    .Include(x => x.Shipment)
                    .Where(x => x.Shipment.JobId == entityId)
                    .Select(x => x.Id);

                query = query.Where(x => (x.EntityTypeId == entityTypeId && x.EntityId == entityId)
                    || (x.EntityTypeId == 5 && x.EntityId.HasValue && bls.Contains(x.EntityId.Value))
                    || (x.EntityTypeId == 2 && x.EntityId.HasValue && bookings.Contains(x.EntityId.Value))
                    || (x.EntityTypeId == 11 && x.EntityId.HasValue && invoices.Contains(x.EntityId.Value))
                    || (x.EntityTypeId == 12 && x.EntityId.HasValue && charges.Contains(x.EntityId.Value))
                    || (x.EntityTypeId == 4 && x.EntityId.HasValue && shipmentLegs.Contains(x.EntityId.Value))
                    );
            }
            else
                query = query.Where(x => x.EntityTypeId == entityTypeId
                             && x.EntityId == entityId
                             && !x.IsDeleted);


            var logs = await query.OrderByDescending(x => x.ActivityDateTime)
                    .Select(x => new ActivityLogListItemVm
                    {
                        Id = x.Id,
                        EntityType = x.EntityType.EntityTypeName,
                        EntityId = x.EntityId.Value,

                        ActivityCategory = x.ActivityCategory.ActivityCategoryName,
                        ActivityType = x.ActivityType,

                        EventTypeName = x.EventType != null ? x.EventType.EventTypeName : null,

                        Title = x.Title,
                        Description = x.Description,
                        ReferenceNo = x.ReferenceNo,

                        ActivityDateTime = x.ActivityDateTime,
                        ActivityDateLocal = DateHelper.getSolarDate10(x.ActivityDateTime),

                        //FromStatusName = x.FromStatus,
                        //ToStatusName = x.ToStatusTable,

                        FieldName = x.FieldName,
                        OldValue = x.OldValue,
                        NewValue = x.NewValue,
                        PerformedByUserName = x.PerformedByUser.FullName,
                        Severity = x.Severity,
                        IconCssClass = x.IconCssClass,
                        VisibleForCustomers = x.VisibleForCustomers,
                        ExternalReferenceNo = x.ExternalReferenceNo,
                        IsSystemGenerated = x.IsSystemGenerated,
                    })
                    .ToListAsync(cancellationToken);

            return logs;
        }




        public async Task<ActivityLogDetailsVm?> GetByIdAsync(
               Guid id,
               CancellationToken cancellationToken = default)
        {
            return await _context.ActivityLogs
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new ActivityLogDetailsVm
                {
                    Id = x.Id,

                    // در معماری شما برای لاگ‌های قابل ویرایش،
                    // EntityTypeId و EntityId باید مقدار داشته باشند.
                    EntityTypeId = x.EntityTypeId ?? 0,
                    EntityId = x.EntityId ?? Guid.Empty,

                    EventTypeId = x.EventTypeId,
                    Title = x.Title,
                    Description = x.Description,
                    ActivityDateTime = x.ActivityDateTime,
                    VisibleForCustomers = x.VisibleForCustomers,
                    ExternalReferenceNo = x.ExternalReferenceNo,
                    InternalNote = x.InternalNote

                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            UpdateActivityLogRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var activityLog = await _context.ActivityLogs
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (activityLog is null)
            {
                throw new KeyNotFoundException(
                    $"Activity log with Id '{request.Id}' was not found.");
            }

            /*
             * توصیه مهم:
             * لاگ‌های سیستم و Audit/History معمولاً نباید از رابط کاربری
             * قابل تغییر باشند؛ وگرنه اعتبار Audit Trail از بین می‌رود.
             *
             * اگر طبق Rule پروژه شما باید امکان ویرایش System Generated Log
             * نیز وجود داشته باشد، این بلوک را حذف کنید.
             */
            if (activityLog.IsSystemGenerated)
            {
                throw new InvalidOperationException(
                    "System generated activity logs cannot be edited.");
            }

            // اگر EventType انتخاب شده، وجود و فعال بودن آن بررسی می‌شود.
            if (request.EventTypeId.HasValue)
            {
                var eventTypeExists = await _context.EventTypes
                    .AnyAsync(x =>
                        x.Id == request.EventTypeId.Value &&
                        x.IsActive,
                        cancellationToken);

                if (!eventTypeExists)
                {
                    throw new InvalidOperationException(
                        "The selected event type does not exist or is inactive.");
                }
            }
            /*
             * عمداً EntityTypeId و EntityId در UpdateActivityLogRequest نیستند.
             * یک Log پس از ایجاد نباید به Entity دیگری منتقل شود.
             */
            activityLog.EventTypeId = request.EventTypeId;
            activityLog.Title = NormalizeNullableText(request.Title);
            activityLog.Description = NormalizeNullableText(request.Description);
            activityLog.ActivityDateTime = request.ActivityDateTime;
            activityLog.VisibleForCustomers = request.VisibleForCustomers;
            activityLog.UpdatedAt = DateTime.Now;
            activityLog.EditUserId = _currentUserService.UserId;
            activityLog.ExternalReferenceNo = request.ExternalReferenceNo;
            activityLog.InternalNote = request.InternalNote;
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var activityLog = await _context.ActivityLogs
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (activityLog is null)
            {
                throw new KeyNotFoundException(
                    $"Activity log with Id '{id}' was not found.");
            }

            /*
             * مشابه Update، برای حفاظت از System/Audit logs.
             * در صورت نیاز کسب‌وکاری می‌توانید این Rule را حذف یا
             * به Permission-based authorization تبدیل کنید.
             */
            if (activityLog.IsSystemGenerated)
            {
                throw new InvalidOperationException(
                    "System generated activity logs cannot be deleted.");
            }
            // Soft Delete
            activityLog.IsDeleted = true;
            activityLog.DeleteUserId = _currentUserService.UserId;
            activityLog.DeletedAt = DateTime.Now;
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<List<EventTypeLookupVm>> GetEventTypesLookupAsync(int entityTypeId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.EventTypes
                .AsNoTracking();

            switch (entityTypeId)
            {
                case 1:
                    break;
                case 2:  //Booking
                    query = query.Where(x => x.CategoryId == 3);
                    break;
                case 4:  //Shipment Leg
                    query = query.Where(x => x.CategoryId == 2);
                    break;
                case 5:  //BL
                    query = query.Where(x => x.CategoryId == 4);
                    break;
                case 11:  //Invoice
                    query = query.Where(x => x.CategoryId == 5);
                    break;
                default:
                    break;
            }

            return await query
                .Where(x => x.IsActive)
                .OrderBy(x => x.EventTypeName)
                .Select(x => new EventTypeLookupVm
                {
                    Id = x.Id,
                    EventTypeName = x.EventTypeName
                })
                .ToListAsync(cancellationToken);
        }

        private static string? NormalizeNullableText(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        public async Task<List<RecentActivityViewModel>> GetRecentActivitiesAsync(
            int take = 8,
            CancellationToken ct = default)
        {
            // فیلتر Global Query Filter ایزدیلیتد روی ActivityLog به‌صورت خودکار اعمال می‌شود
            var raw = await _context.ActivityLogs
                .AsNoTracking()
                .Include(a => a.PerformedByUser)
                .Include(a => a.Location)
                .Where(a => a.VisibleForCustomers == false)   // لاگ‌های داخلی
                .OrderByDescending(a => a.ActivityDateTime)
                .Take(take)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.ReferenceNo,
                    a.ActivityDateTime,
                    a.Severity,
                    a.IconCssClass,
                    a.IsSystemGenerated,
                    a.SourceSystem,
                    PerformedByName = a.PerformedByUser != null
                        ? a.PerformedByUser.FullName : null,
                    LocationName = a.Location != null ? a.Location.LocationName : null
                })
                .ToListAsync(ct);

            return raw.Select(a => new RecentActivityViewModel
            {
                Id = a.Id,
                Title = a.Title ?? "Activity",
                Description = a.Description,
                ReferenceNo = a.ReferenceNo,
                ActivityDateTime = a.ActivityDateTime,
                Severity = string.IsNullOrWhiteSpace(a.Severity)
                    ? "Primary" : a.Severity,
                IconCssClass = string.IsNullOrWhiteSpace(a.IconCssClass)
                    ? IconForSeverity(a.Severity) : a.IconCssClass,
                PerformedByUserName = a.IsSystemGenerated
                    ? "System"
                    : a.PerformedByName,
                LocationName = a.LocationName,
                SourceSystem = a.SourceSystem
            }).ToList();
        }

        private static string IconForSeverity(string? severity) => severity?.ToUpperInvariant() switch
        {
            "INFO" => "bi-info-circle",
            "SUCCESS" => "bi-check-circle",
            "WARNING" => "bi-exclamation-triangle",
            "DANGER" => "bi-x-circle",
            _ => "bi-activity"
        };
    }
}
