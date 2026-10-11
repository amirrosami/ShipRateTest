using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.Notif;
using ShipRate.Application.Interfaces.Notif;
using ShipRate.Domain.Entities.Logs;
using ShipRate.Domain.Entities.Notifs;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Notif
{
    public sealed class NotificationRecipientResolver : INotificationRecipientResolver
    {
        private readonly ApplicationDbContext _db;

        public NotificationRecipientResolver(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Guid>> ResolveAsync(
            ActivityLog activityLog,
            NotificationRuleDto rule,
            CancellationToken ct = default)
        {
            if (activityLog == null)
            {
                throw new ArgumentNullException(nameof(activityLog));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            if (!rule.ShouldNotify || rule.RecipientModeId == (int)EnumNotificationRecipientMode.None)
            {
                return new List<Guid>();
            }

            var recipientUserIds = new List<Guid>();
            switch (rule.RecipientModeId)
            {
                case (int)EnumNotificationRecipientMode.Actor:
                    AddActor(recipientUserIds, activityLog);
                    break;

                case (int)EnumNotificationRecipientMode.Admins:
                    recipientUserIds.AddRange(await GetAdminUserIdsAsync(ct));
                    break;

                case (int)EnumNotificationRecipientMode.SpecificUsers:
                    recipientUserIds.AddRange(rule.UserIds);
                    break;

                case (int)EnumNotificationRecipientMode.UserTypes:
                    recipientUserIds.AddRange(await GetUserIdsByUserTypesAsync(rule.UserTypeIds, ct));
                    break;

                case (int)EnumNotificationRecipientMode.EntityRelatedUsers:
                    recipientUserIds.AddRange(await GetEntityRelatedUserIdsAsync(activityLog, ct));
                    break;

                case (int)EnumNotificationRecipientMode.Mixed:
                    AddActor(recipientUserIds, activityLog);

                    recipientUserIds.AddRange(rule.UserIds);
                    recipientUserIds.AddRange(await GetUserIdsByUserTypesAsync(rule.UserTypeIds, ct));
                    recipientUserIds.AddRange(await GetEntityRelatedUserIdsAsync(activityLog, ct));
                    break;
            }

            if (rule.ExcludeActor)
            {
                recipientUserIds.Remove(activityLog.CreateUserId);
            }


            return recipientUserIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();
        }

        private static void AddActor(List<Guid> recipientUserIds, ActivityLog activityLog)
        {
            recipientUserIds.Add(activityLog.CreateUserId);
        }

        private async Task<List<Guid>> GetAdminUserIdsAsync(CancellationToken ct)
        {
            //return await _db.Users
            //    .AsNoTracking()
            //    .Where(x => x.IsActive && x.UserRoles.Any(ur => ur.Role.Name == "Admin"))
            //    .Select(x => x.Id)
            //    .ToListAsync(ct);


            var adminRoleId = await _db.Roles.Where(x => x.Name == "Admin").Select(x => x.Id).FirstOrDefaultAsync();

            return await _db.UserRoles
                .AsNoTracking()
                .Where(x => x.RoleId == adminRoleId)
                .Select(x => x.UserId)
                .ToListAsync(ct);
        }

        private async Task<List<Guid>> GetUserIdsByUserTypesAsync(
            List<int> userTypeIds,
            CancellationToken ct)
        {
            if (userTypeIds == null || userTypeIds.Count == 0)
            {
                return new List<Guid>();
            }

            return await _db.Users
                .AsNoTracking()
                .Where(x => x.IsActive && userTypeIds.Contains(x.UserTypeId.Value))
                .Select(x => x.Id)
                .ToListAsync(ct);
        }

        private async Task<List<Guid>> GetEntityRelatedUserIdsAsync(
            ActivityLog activityLog,
            CancellationToken ct)
        {
            if (!activityLog.EntityId.HasValue)
            {
                return new List<Guid>();
            }

            var entityTypeId = activityLog.EntityTypeId;

            if (entityTypeId == 1)  //Job
            {
                return await GetJobRelatedUserIdsAsync(activityLog.EntityId.Value, ct);
            }

            //if (entityType.Equals("Order", StringComparison.OrdinalIgnoreCase))
            //{
            //    return await GetOrderRelatedUserIdsAsync(activityLog.EntityId.Value, ct);
            //}

            //if (entityType.Equals("Ticket", StringComparison.OrdinalIgnoreCase))
            //{
            //    return await GetTicketRelatedUserIdsAsync(activityLog.EntityId.Value, ct);
            //}

            return new List<Guid>();
        }
        private async Task<List<Guid>> GetJobRelatedUserIdsAsync(Guid jobId, CancellationToken ct)
        {
            return await _db.Jobs
                .AsNoTracking()
                .Where(x => x.Id == jobId)
                .SelectMany(x => new Guid?[]
                {
                x.CreateUserId,
                x.EditUserId,
                x.SalesUserId,
                //x.CustomerUserId
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToListAsync(ct);
        }
    }
}
