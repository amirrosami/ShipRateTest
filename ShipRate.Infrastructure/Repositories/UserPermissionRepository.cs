using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    // Infrastructure/Repositories/UserPermissionRepository.cs

    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public UserPermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserPermission?> GetUserPermissionAsync(Guid userId, int permissionId)
        {
            return await _context.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
        }

        public async Task AddAsync(UserPermission userPermission)
        {
            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(UserPermission userPermission)
        {
            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();
        }

        // 🆕 دریافت Permissionهای User با Include کردن جزئیات
        public async Task<List<UserPermission>> GetUserPermissionsWithDetailsAsync(Guid userId)
        {
            return await _context.UserPermissions
                .Include(up => up.Permission) // 👈 مهم: برای دسترسی به Permission.Name و غیره
                .Where(up => up.UserId == userId)
                .ToListAsync();
        }
    }
}
