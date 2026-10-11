using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;
using System.Security.Claims;

namespace ShipRate.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionGroupPermissionRepository _permissionGroupPermissionRepository;
        private readonly IUserPermissionGroupRepository _userPermissionGroupRepository;
        private readonly IUserPermissionRepository _userPermissionRepository;

        public PermissionService(ApplicationDbContext context, IPermissionGroupPermissionRepository permissionGroupPermissionRepository, IUserPermissionGroupRepository userPermissionGroupRepository, IUserPermissionRepository userPermissionRepository)
        {
            _context = context;
            _permissionGroupPermissionRepository = permissionGroupPermissionRepository;
            _userPermissionGroupRepository = userPermissionGroupRepository;
            _userPermissionRepository = userPermissionRepository;
        }

        public async Task<List<Claim>> GetUserPermissionClaimsAsync(Guid userId)
        {
            // خواندن Permissionهای فعال کاربر
            var permissions = await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up => up.UserId == userId && up.Permission.IsActive)
                .Select(up => up.Permission.Name)
                .ToListAsync();

            // تبدیل به Claim
            return permissions
                .Select(p => new Claim("Permission", p))
                .ToList();
        }

        public async Task<List<string>> GetUserPermissionNamesAsync(Guid userId)
        {
            return await _context.UserPermissions
                .Include(up => up.Permission)
                .Where(up => up.UserId == userId && up.Permission.IsActive)
                .Select(up => up.Permission.Name)
                .ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
        {
            return await _context.UserPermissions
                .Include(up => up.Permission)
                .AnyAsync(up =>
                    up.UserId == userId &&
                    up.Permission.Name == permissionName &&
                    up.Permission.IsActive);
        }
        public async Task<List<PermissionDto>> GetAllPermissionsAsync()
        {
            var permissions = await _context.Permissions.ToListAsync();

            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                DisplayName = p.DisplayName,
                Category = p.Category,
                Description = p.Description
            }).ToList();
        }

        public async Task<List<PermissionDto>> GetUserPermissionsAsync(Guid userId)
        {
            var userPermissions = await _context.UserPermissions.Include(up => up.Permission)
                                        .Where(up => up.UserId == userId && up.Permission.IsActive)
                                        .ToListAsync();

            return userPermissions.Select(up => new PermissionDto
            {
                Id = up.Permission.Id,
                Name = up.Permission.Name,
                DisplayName = up.Permission.DisplayName,
                Category = up.Permission.Category,
                Description = up.Permission.Description
            }).ToList();
        }


        public async Task<List<PermissionDto>> GetEffectivePermissionsAsync(Guid userId)
        {
            // 1️⃣ Permissionهای مستقیم User
            var directPermissions = await _userPermissionRepository
                .GetUserPermissionsWithDetailsAsync(userId);

            // 2️⃣ Groupهای User
            var userGroups = await _userPermissionGroupRepository
                .GetUserGroupsAsync(userId);

            var groupIds = userGroups
                .Select(g => g.PermissionGroupId)
                .ToList();

            // 3️⃣ Permissionهای Group
            var groupPermissions = groupIds.Any()
                ? await _permissionGroupPermissionRepository.GetGroupPermissionsAsync(groupIds)
                : new List<PermissionGroupPermission>();

            // 4️⃣ Union + Distinct
            var permissions = directPermissions
                .Select(up => up.Permission)
                .Concat(groupPermissions.Select(gp => gp.Permission))
                .Where(p => p.IsActive)
                .GroupBy(p => p.Id)
                .Select(g => g.First())
                .ToList();

            // 5️⃣ Map to DTO
            return permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                DisplayName = p.DisplayName,
                Category = p.Category,
                Description = p.Description
            }).ToList();
        }


    }
}
