using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Notif;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Notif;
using ShipRate.Domain.Entities.Notifs;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Notif
{
    public sealed class NotificationRuleService : INotificationRuleService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        public NotificationRuleService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        public async Task<NotificationRuleDto?> GetRuleForActivityAsync(
            string activityType,
            int entityTypeId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(activityType) || entityTypeId == 0)
            {
                return null;
            }

            activityType = activityType.Trim();

            return await _db.NotificationRules
                .Include(x => x.EntityType)
                .AsNoTracking()
                .Where(x =>
                    x.ActivityType == activityType &&
                    x.EntityTypeId == entityTypeId)
                .Select(x => new NotificationRuleDto
                {
                    ActivityType = x.ActivityType,
                    EntityType = x.EntityType.EntityTypeName,
                    ShouldNotify = x.ShouldNotify,
                    RecipientMode = x.RecipientMode,
                    UserIds = x.Users
                        .Select(u => u.UserId)
                        .ToList(),
                    UserTypeIds = x.UserTypes
                        .Select(ut => ut.UserTypeId)
                        .ToList(),
                    TitleTemplate = x.TitleTemplate,
                    MessageTemplate = x.MessageTemplate,
                    TargetUrlTemplate = x.TargetUrlTemplate
                })
                .FirstOrDefaultAsync(ct);
        }
        public async Task<PagedResultDto<NotificationRuleIndexDto>> GetListAsync(NotificationRuleGetListInput input)
        {
            var query = _db.NotificationRules
                .AsNoTracking()
                .Include(x => x.EntityType)
                .Include(x => x.RecipientMode)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(input.ActivityType))
            {
                var activityType = input.ActivityType.Trim();
                query = query.Where(x => x.ActivityType.Contains(activityType));
            }

            if (input.EntityTypeId.HasValue)
            {
                query = query.Where(x => x.EntityTypeId == input.EntityTypeId.Value);
            }

            if (input.RecipientModeId.HasValue)
            {
                query = query.Where(x => x.RecipientModeId == input.RecipientModeId.Value);
            }

            //if (input.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == input.IsActive.Value);
            //}

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new NotificationRuleIndexDto
                {
                    Id = x.Id,
                    ActivityType = x.ActivityType,
                    EntityTypeName = x.EntityType.EntityTypeName,
                    RecipientModeName = x.RecipientMode.RecipientModeName,
                    ShouldNotify = x.ShouldNotify,
                    //IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return new PagedResultDto<NotificationRuleIndexDto>(items, totalCount, 1, 10);
        }

        public async Task<NotificationRuleCreateDto> GetCreateDtoAsync()
        {
            var dto = new NotificationRuleCreateDto();
            await FillLookupsAsync(dto);
            return dto;
        }

        public async Task CreateAsync(NotificationRuleCreateDto input)
        {
            await NormalizeSelectionsAsync(input);
            await ValidateAsync(input.RecipientModeId, input.SelectedUserIds, input.SelectedUserTypeIds);

            var currentUserId = GetCurrentUserId();

            var entity = new NotificationRule
            {
                Id = Guid.CreateVersion7(),
                ActivityType = input.ActivityType.Trim(),
                EntityTypeId = input.EntityTypeId,
                ShouldNotify = input.ShouldNotify,
                RecipientModeId = input.RecipientModeId,
                TitleTemplate = input.TitleTemplate.Trim(),
                MessageTemplate = input.MessageTemplate.Trim(),
                TargetUrlTemplate = input.TargetUrlTemplate?.Trim(),
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            entity.Users = input.SelectedUserIds
                .Distinct()
                .Select(userId => new NotificationRuleUser
                {
                    Id = Guid.CreateVersion7(),
                    NotificationRuleId = entity.Id,
                    UserId = userId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId
                })
                .ToList();

            entity.UserTypes = input.SelectedUserTypeIds
                .Distinct()
                .Select(userTypeId => new NotificationRuleUserType
                {
                    Id = Guid.CreateVersion7(),
                    NotificationRuleId = entity.Id,
                    UserTypeId = userTypeId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId
                })
                .ToList();

            _db.NotificationRules.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<NotificationRuleEditDto?> GetEditDtoAsync(Guid id)
        {
            var entity = await _db.NotificationRules
                .AsNoTracking()
                .Include(x => x.Users)
                .Include(x => x.UserTypes)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            var dto = new NotificationRuleEditDto
            {
                Id = entity.Id,
                ActivityType = entity.ActivityType,
                EntityTypeId = entity.EntityTypeId,
                ShouldNotify = entity.ShouldNotify,
                RecipientModeId = entity.RecipientModeId,
                TitleTemplate = entity.TitleTemplate,
                MessageTemplate = entity.MessageTemplate,
                TargetUrlTemplate = entity.TargetUrlTemplate,
                SelectedUserIds = entity.Users
                    .Where(x => !x.IsDeleted)
                    .Select(x => x.UserId)
                    .ToList(),
                SelectedUserTypeIds = entity.UserTypes
                    .Where(x => !x.IsDeleted)
                    .Select(x => x.UserTypeId)
                    .ToList()
            };

            await FillLookupsAsync(dto);
            return dto;
        }

        public async Task UpdateAsync(NotificationRuleEditDto input)
        {
            await NormalizeSelectionsAsync(input);
            await ValidateAsync(input.RecipientModeId, input.SelectedUserIds, input.SelectedUserTypeIds);

            var entity = await _db.NotificationRules
                .Include(x => x.Users)
                .Include(x => x.UserTypes)
                .FirstOrDefaultAsync(x => x.Id == input.Id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Notification rule not found.");

            var currentUserId = GetCurrentUserId();
            var now = DateTime.Now;

            entity.ActivityType = input.ActivityType.Trim();
            entity.EntityTypeId = input.EntityTypeId;
            entity.ShouldNotify = input.ShouldNotify;
            entity.RecipientModeId = input.RecipientModeId;
            entity.TitleTemplate = input.TitleTemplate.Trim();
            entity.MessageTemplate = input.MessageTemplate.Trim();
            entity.TargetUrlTemplate = input.TargetUrlTemplate?.Trim();
            entity.UpdatedAt = now;
            entity.EditUserId = currentUserId;

            var selectedUserIds = (input.SelectedUserIds ?? new List<Guid>())
                .Distinct()
                .ToHashSet();

            var selectedUserTypeIds = (input.SelectedUserTypeIds ?? new List<int>())
                .Distinct()
                .ToHashSet();

            SyncUsers(entity, selectedUserIds, currentUserId, now);
            SyncUserTypes(entity, selectedUserTypeIds, currentUserId, now);

            await _db.SaveChangesAsync();
        }

        private static void SyncUsers(
            NotificationRule entity,
            HashSet<Guid> selectedUserIds,
            Guid currentUserId,
            DateTime now)
        {
            foreach (var item in entity.Users)
            {
                if (selectedUserIds.Contains(item.UserId))
                {
                    if (item.IsDeleted)
                    {
                        item.IsDeleted = false;
                        item.UpdatedAt = now;
                        item.EditUserId = currentUserId;
                    }
                }
                else if (!item.IsDeleted)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = now;
                    item.EditUserId = currentUserId;
                }
            }

            var existingUserIds = entity.Users
                .Where(x => !x.IsDeleted)
                .Select(x => x.UserId)
                .ToHashSet();

            var deletedUserMap = entity.Users
                .Where(x => x.IsDeleted)
                .ToDictionary(x => x.UserId, x => x);

            foreach (var userId in selectedUserIds)
            {
                if (existingUserIds.Contains(userId))
                    continue;

                if (deletedUserMap.TryGetValue(userId, out var deletedItem))
                {
                    deletedItem.IsDeleted = false;
                    deletedItem.UpdatedAt = now;
                    deletedItem.EditUserId = currentUserId;
                    continue;
                }

                entity.Users.Add(new NotificationRuleUser
                {
                    Id = Guid.CreateVersion7(),
                    NotificationRuleId = entity.Id,
                    UserId = userId,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreateUserId = currentUserId
                });
            }
        }

        private static void SyncUserTypes(
            NotificationRule entity,
            HashSet<int> selectedUserTypeIds,
            Guid currentUserId,
            DateTime now)
        {
            foreach (var item in entity.UserTypes)
            {
                if (selectedUserTypeIds.Contains(item.UserTypeId))
                {
                    if (item.IsDeleted)
                    {
                        item.IsDeleted = false;
                        item.UpdatedAt = now;
                        item.EditUserId = currentUserId;
                    }
                }
                else if (!item.IsDeleted)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = now;
                    item.EditUserId = currentUserId;
                }
            }

            var existingUserTypeIds = entity.UserTypes
                .Where(x => !x.IsDeleted)
                .Select(x => x.UserTypeId)
                .ToHashSet();

            var deletedUserTypeMap = entity.UserTypes
                .Where(x => x.IsDeleted)
                .ToDictionary(x => x.UserTypeId, x => x);

            foreach (var userTypeId in selectedUserTypeIds)
            {
                if (existingUserTypeIds.Contains(userTypeId))
                    continue;

                if (deletedUserTypeMap.TryGetValue(userTypeId, out var deletedItem))
                {
                    deletedItem.IsDeleted = false;
                    deletedItem.UpdatedAt = now;
                    deletedItem.EditUserId = currentUserId;
                    continue;
                }

                entity.UserTypes.Add(new NotificationRuleUserType
                {
                    Id = Guid.CreateVersion7(),
                    NotificationRuleId = entity.Id,
                    UserTypeId = userTypeId,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreateUserId = currentUserId
                });
            }
        }

        public async Task<NotificationRuleDetailsDto?> GetDetailsAsync(Guid id)
        {
            var entity = await _db.NotificationRules
                .AsNoTracking()
                .Include(x => x.EntityType)
                .Include(x => x.RecipientMode)
                .Include(x => x.Users).ThenInclude(x => x.User)
                .Include(x => x.UserTypes).ThenInclude(x => x.UserType)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            return new NotificationRuleDetailsDto
            {
                Id = entity.Id,
                ActivityType = entity.ActivityType,
                EntityTypeId = entity.EntityTypeId,
                EntityTypeName = entity.EntityType.EntityTypeName,
                ShouldNotify = entity.ShouldNotify,
                RecipientModeId = entity.RecipientModeId,
                RecipientModeName = entity.RecipientMode.RecipientModeName,
                TitleTemplate = entity.TitleTemplate,
                MessageTemplate = entity.MessageTemplate,
                TargetUrlTemplate = entity.TargetUrlTemplate,
                Users = entity.Users
                    .Where(x => !x.IsDeleted)
                    .Select(x => x.User.FullName)
                    .ToList(),
                UserTypes = entity.UserTypes
                    .Where(x => !x.IsDeleted)
                    .Select(x => x.UserType.UserTypeName)
                    .ToList(),
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public async Task<NotificationRuleDeleteDto?> GetDeleteDtoAsync(Guid id)
        {
            return await _db.NotificationRules
                .AsNoTracking()
                .Include(x => x.EntityType)
                .Include(x => x.RecipientMode)
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new NotificationRuleDeleteDto
                {
                    Id = x.Id,
                    ActivityType = x.ActivityType,
                    EntityTypeName = x.EntityType.EntityTypeName,
                    RecipientModeName = x.RecipientMode.RecipientModeName
                })
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.NotificationRules
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Notification rule not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = GetCurrentUserId();

            await _db.SaveChangesAsync();
        }

        private async Task FillLookupsAsync(NotificationRuleCreateDto dto)
        {
            dto.EntityTypes = await _db.EntityTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EntityTypeName
                })
                .ToListAsync();

            dto.RecipientModes = await _db.NotificationRecipientModes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RecipientModeName
                })
                .ToListAsync();

            dto.Users = await _db.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();

            dto.UserTypes = await _db.UserTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserTypeName
                })
                .ToListAsync();
        }

        private async Task FillLookupsAsync(NotificationRuleEditDto dto)
        {
            dto.EntityTypes = await _db.EntityTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EntityTypeName
                })
                .ToListAsync();

            dto.RecipientModes = await _db.NotificationRecipientModes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RecipientModeName
                })
                .ToListAsync();

            dto.Users = await _db.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();

            dto.UserTypes = await _db.UserTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserTypeName
                })
                .ToListAsync();
        }

        private async Task NormalizeSelectionsAsync(NotificationRuleCreateDto input)
        {
            var specificUsersModeId = await GetRecipientModeIdByNameAsync("SpecificUsers");
            var userTypesModeId = await GetRecipientModeIdByNameAsync("UserTypes");

            if (input.RecipientModeId != specificUsersModeId)
                input.SelectedUserIds.Clear();

            if (input.RecipientModeId != userTypesModeId)
                input.SelectedUserTypeIds.Clear();
        }

        private async Task NormalizeSelectionsAsync(NotificationRuleEditDto input)
        {
            var specificUsersModeId = await GetRecipientModeIdByNameAsync("SpecificUsers");
            var userTypesModeId = await GetRecipientModeIdByNameAsync("UserTypes");

            if (input.RecipientModeId != specificUsersModeId)
                input.SelectedUserIds.Clear();

            if (input.RecipientModeId != userTypesModeId)
                input.SelectedUserTypeIds.Clear();
        }

        private async Task ValidateAsync(int recipientModeId, List<Guid> selectedUserIds, List<int> selectedUserTypeIds)
        {
            var specificUsersModeId = await GetRecipientModeIdByNameAsync("SpecificUsers");
            var userTypesModeId = await GetRecipientModeIdByNameAsync("UserTypes");

            if (recipientModeId == specificUsersModeId && !selectedUserIds.Any())
                throw new Exception("At least one user must be selected.");

            if (recipientModeId == userTypesModeId && !selectedUserTypeIds.Any())
                throw new Exception("At least one user type must be selected.");
        }

        private async Task<int> GetRecipientModeIdByNameAsync(string name)
        {
            return await _db.NotificationRecipientModes
                .AsNoTracking()
                .Where(x => x.RecipientModeName == name && x.IsActive && !x.IsDeleted)
                .Select(x => x.Id)
                .FirstAsync();
        }

        private Guid GetCurrentUserId()
        {
            var userId = _currentUserService.UserId;
            return userId;
        }
    }
}
