using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class PermissionGroupService : IPermissionGroupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserPermissionGroupRepository _userPermissionGroupRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public PermissionGroupService(ApplicationDbContext context, IUserPermissionGroupRepository userPermissionGroupRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _userPermissionGroupRepository = userPermissionGroupRepository;
        }

        public async Task<List<PermissionGroupDto>> GetAllGroupsAsync()
        {
            return await _context.PermissionGroups
                .Select(pg => new PermissionGroupDto
                {
                    Id = pg.Id,
                    Name = pg.Name,
                    Description = pg.Description,
                    IsActive = pg.IsActive,
                    PermissionCount = pg.PermissionGroupPermissions.Count
                })
                .ToListAsync();
        }

        public async Task<PermissionGroupDetailDto?> GetGroupDetailAsync(int groupId)
        {
            var group = await _context.PermissionGroups
                .Include(pg => pg.PermissionGroupPermissions)
                    .ThenInclude(pgp => pgp.Permission)
                .FirstOrDefaultAsync(pg => pg.Id == groupId);

            if (group == null)
                return null;

            return new PermissionGroupDetailDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                IsActive = group.IsActive,
                Permissions = group.PermissionGroupPermissions
                    .Select(pgp => new PermissionDto
                    {
                        Id = pgp.Permission.Id,
                        Name = pgp.Permission.Name,
                        DisplayName = pgp.Permission.DisplayName,
                        Category = pgp.Permission.Category
                    })
                    .ToList()
            };
        }

        public async Task<bool> CreateGroupAsync(CreatePermissionGroupDto dto)
        {
            // بررسی تکراری بودن نام
            if (await _context.PermissionGroups.AnyAsync(pg => pg.Name == dto.Name))
                return false;

            var group = new PermissionGroup
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.PermissionGroups.Add(group);
            await _context.SaveChangesAsync();

            // Assign Permissions
            foreach (var permissionId in dto.PermissionIds)
            {
                _context.PermissionGroupPermissions.Add(new PermissionGroupPermission
                {
                    PermissionGroupId = group.Id,
                    PermissionId = permissionId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGroupAsync(UpdatePermissionGroupDto dto)
        {
            var group = await _context.PermissionGroups
                .Include(pg => pg.PermissionGroupPermissions)
                .FirstOrDefaultAsync(pg => pg.Id == dto.Id);

            if (group == null)
                return false;

            group.Name = dto.Name;
            group.Description = dto.Description;
            group.IsActive = dto.IsActive;
            group.UpdatedAt = DateTime.Now;

            // حذف Permissionهای قبلی
            _context.PermissionGroupPermissions.RemoveRange(
                group.PermissionGroupPermissions);

            // اضافه کردن جدید
            foreach (var permissionId in dto.PermissionIds)
            {
                _context.PermissionGroupPermissions.Add(new PermissionGroupPermission
                {
                    PermissionGroupId = group.Id,
                    PermissionId = permissionId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            var group = await _context.PermissionGroups.FindAsync(groupId);
            if (group == null)
                return false;

            _context.PermissionGroups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleGroupStatusAsync(int groupId)
        {
            var group = await _context.PermissionGroups.FindAsync(groupId);
            if (group == null)
                return false;

            group.IsActive = !group.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // 🆕 Assign Group به User
        public async Task<bool> AssignGroupToUserAsync(AssignPermissionGroupToUserDto dto)
        {
            // بررسی User وجود داشته باشد
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (user == null)
                return false;

            // بررسی PermissionGroup وجود داشته باشد
            var group = await _context.PermissionGroups.FindAsync(dto.PermissionGroupId);
            if (group == null || !group.IsActive)
                return false;

            // بررسی Duplicate
            if (await _userPermissionGroupRepository.ExistsAsync(dto.UserId, dto.PermissionGroupId))
                return false;

            var userPermissionGroup = new UserPermissionGroup
            {
                UserId = dto.UserId,
                PermissionGroupId = dto.PermissionGroupId,
                AssignedAt = DateTime.Now
            };

            await _userPermissionGroupRepository.AddAsync(userPermissionGroup);
            return true;
        }

        // 🆕 Remove Group از User
        public async Task<bool> RemoveGroupFromUserAsync(int permissionGroupId, Guid userId)
        {
            var userPermissionGroup = await _userPermissionGroupRepository
                .GetAsync(userId, permissionGroupId);

            if (userPermissionGroup == null)
                return false;

            await _userPermissionGroupRepository.RemoveAsync(userPermissionGroup);
            return true;
        }

        // 🆕 دریافت گروه‌های یک User
        public async Task<List<UserPermissionGroupDto>> GetUserGroupsAsync(Guid userId)
        {
            var userGroups = await _userPermissionGroupRepository.GetUserGroupsAsync(userId);

            return userGroups.Select(ug => new UserPermissionGroupDto
            {
                UserId = ug.UserId,
                UserName = ug.User?.UserName ?? "",
                Email = ug.User?.Email ?? "",
                PermissionGroupId = ug.PermissionGroupId,
                PermissionGroupName = ug.PermissionGroup?.Name ?? "",
                AssignedAt = ug.AssignedAt
            }).ToList();
        }

        // 🆕 دریافت کاربران یک Group
        public async Task<List<UserPermissionGroupDto>> GetGroupUsersAsync(int permissionGroupId)
        {
            var groupUsers = await _userPermissionGroupRepository
                .GetGroupUsersAsync(permissionGroupId);

            return groupUsers.Select(gu => new UserPermissionGroupDto
            {
                UserId = gu.UserId,
                UserName = gu.User?.UserName ?? "",
                Email = gu.User?.Email ?? "",
                PermissionGroupId = gu.PermissionGroupId,
                PermissionGroupName = gu.PermissionGroup?.Name ?? "",
                AssignedAt = gu.AssignedAt
            }).ToList();
        }


    }
}
