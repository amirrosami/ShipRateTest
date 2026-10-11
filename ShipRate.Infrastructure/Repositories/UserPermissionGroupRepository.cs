using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;
namespace ShipRate.Infrastructure.Repositories
{
    public class UserPermissionGroupRepository : IUserPermissionGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public UserPermissionGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserPermissionGroup?> GetAsync(Guid userId, int permissionGroupId)
        {
            return await _context.UserPermissionGroups
                .Include(upg => upg.PermissionGroup)
                .Include(upg => upg.User)
                .FirstOrDefaultAsync(upg =>
                    upg.UserId == userId &&
                    upg.PermissionGroupId == permissionGroupId);
        }

        public async Task<List<UserPermissionGroup>> GetUserGroupsAsync(Guid userId)
        {
            return await _context.UserPermissionGroups
                .Include(upg => upg.PermissionGroup)
                .Where(upg => upg.UserId == userId && !upg.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<UserPermissionGroup>> GetGroupUsersAsync(int permissionGroupId)
        {
            return await _context.UserPermissionGroups
                .Include(upg => upg.User)
                .Where(upg => upg.PermissionGroupId == permissionGroupId && !upg.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(UserPermissionGroup userPermissionGroup)
        {
            await _context.UserPermissionGroups.AddAsync(userPermissionGroup);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(UserPermissionGroup userPermissionGroup)
        {
            _context.UserPermissionGroups.Remove(userPermissionGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, int permissionGroupId)
        {
            return await _context.UserPermissionGroups
                .AnyAsync(upg =>
                    upg.UserId == userId &&
                    upg.PermissionGroupId == permissionGroupId);
        }
    }
}

