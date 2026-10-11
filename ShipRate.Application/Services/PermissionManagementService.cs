using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    // Application/Services/PermissionManagementService.cs
    public class PermissionManagementService : IPermissionManagementService
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuditService _auditLogService;

        public PermissionManagementService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuditService auditLogService)
        {
            _context = context;
            _userManager = userManager;
            //_signInManager = signInManager;
            _auditLogService = auditLogService;
        }

        public async Task<List<UserListItemDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .Select(u => new
                {
                    User = u,
                    PermissionsCount = _context.UserPermissions
                        .Count(up => up.UserId == u.Id)
                })
                .ToListAsync();

            return users.Select(x => new UserListItemDto
            {
                UserId = x.User.Id,
                UserName = x.User.UserName ?? "",
                Email = x.User.Email,
                FullName = $"{x.User.FirstName} {x.User.LastName}".Trim(),
                PermissionsCount = x.PermissionsCount,
                IsActive = !x.User.LockoutEnabled ||
                          x.User.LockoutEnd == null ||
                          x.User.LockoutEnd < DateTimeOffset.Now
            }).ToList();
        }

        public async Task<UserPermissionsDto> GetUserPermissionsAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserPermissions)
                    .ThenInclude(up => up.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var allPermissions = await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category).ThenBy(x => x.Id)
                .ThenBy(p => p.DisplayName)
                .ToListAsync();

            var assignedIds = user.UserPermissions
                .Where(x => !x.IsDeleted)
                .Select(up => up.PermissionId)
                .ToList();

            return new UserPermissionsDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AllPermissions = allPermissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Description = p.Description,
                    Category = p.Category,
                    IsActive = p.IsActive
                }).ToList(),
                AssignedPermissionIds = assignedIds
            };
        }

        public async Task<int> AssignPermissionsAsync(Guid userId, List<int> permissionIds, Guid actionUserId)
        {
            permissionIds ??= new List<int>();

            var incomingPermissionIds = permissionIds
                .Distinct()
                .ToHashSet();

            var existingPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .ToListAsync();

            // Soft delete permissions removed from UI
            foreach (var permission in existingPermissions.Where(up => !incomingPermissionIds.Contains(up.PermissionId)))
            {
                permission.IsDeleted = true;
                permission.DeleteUserId = actionUserId;
                permission.DeletedAt = DateTime.Now;
            }

            // Add new permissions
            var existingPermissionIds = existingPermissions
                .Where(up => !up.IsDeleted)
                .Select(up => up.PermissionId)
                .ToHashSet();

            var permissionsToAdd = incomingPermissionIds
                .Except(existingPermissionIds)
                .Select(pid => new UserPermission
                {
                    UserId = userId,
                    PermissionId = pid,
                    AssignedAt = DateTime.Now,
                    AssignedBy = actionUserId,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = actionUserId,
                })
                .ToList();

            if (permissionsToAdd.Any())
                _context.UserPermissions.AddRange(permissionsToAdd);

            return await _context.SaveChangesAsync();
        }
        public async Task<int> RevokeAllPermissionsAsync(Guid userId, string revokedByUserId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException("User Not Found", nameof(userId));

            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .ToListAsync();

            if (userPermissions.Count == 0)
                return 0;

            _context.UserPermissions.RemoveRange(userPermissions);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                revokedByUserId,
                "RevokeAllPermissions",
                "UserPermission",
                userId,
                $"لغو تمام دسترسی‌های کاربر {user.UserName} ({userPermissions.Count} مورد)"
            );

            // ✅ به‌روزرسانی Claims
            await RefreshUserClaimsAsync(user);

            return userPermissions.Count;
        }


        public async Task<int> RevokePermissionsAsync(Guid userId, List<int> permissionIds)
        {
            var toRemove = await _context.UserPermissions
                .Where(up => up.UserId == userId && permissionIds.Contains(up.PermissionId))
                .ToListAsync();

            _context.UserPermissions.RemoveRange(toRemove);
            return await _context.SaveChangesAsync();
        }
        /// <summary>
        /// به‌روزرسانی Claims کاربر بدون نیاز به Login مجدد
        /// </summary>
        private async Task RefreshUserClaimsAsync(ApplicationUser user)
        {
            // ✅ RefreshSignIn خودکار Factory را فراخوانی می‌کند
            //await _signInManager.RefreshSignInAsync(user);
        }
        public async Task<int> AssignPermissionGroupsAsync(
            Guid userId,
            List<int> permissionGroupIds,
            Guid actionUserId)
        {
            permissionGroupIds ??= [];

            var selectedGroupIds = permissionGroupIds
                .Distinct()
                .ToHashSet();

            var existingAssignments = await _context.UserPermissionGroups
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var groupsToRemove = existingAssignments
                .Where(x => !selectedGroupIds.Contains(x.PermissionGroupId))
                .ToList();

            if (groupsToRemove.Any())
            {
                //   _context.UserPermissionGroups.RemoveRange(groupsToRemove);
                foreach (var permission in groupsToRemove)
                {
                    permission.IsDeleted = true;
                    permission.DeletedByUserId = actionUserId;
                    permission.DeletedAt = DateTime.Now;
                }
            }

            var existingGroupIds = existingAssignments
                .Select(x => x.PermissionGroupId)
                .ToHashSet();

            var groupsToAdd = selectedGroupIds
                .Except(existingGroupIds)
                .Select(groupId => new UserPermissionGroup
                {
                    UserId = userId,
                    PermissionGroupId = groupId,
                    AssignedAt = DateTime.Now,
                    AssignedByUserId = actionUserId
                })
                .ToList();

            if (groupsToAdd.Any())
            {
                _context.UserPermissionGroups.AddRange(groupsToAdd);
            }

            return await _context.SaveChangesAsync();
        }














        /// <summary>
        /// توضیحات هر Category
        /// </summary>
        private static string? GetCategoryDescription(string category)
        {
            return category switch
            {
                "Users" => "مدیریت کاربران",
                "Roles" => "مدیریت نقش‌ها",
                "Permissions" => "مدیریت دسترسی‌ها",
                "AuditLogs" => "گزارش‌های ممیزی",
                "SecurityEvents" => "رویدادهای امنیتی",
                "Countries" => "مدیریت کشورها",
                "Cities" => "مدیریت شهرها",
                _ => null
            };
        }
    }
}
