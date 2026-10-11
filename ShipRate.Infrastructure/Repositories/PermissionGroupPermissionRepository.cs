using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class PermissionGroupPermissionRepository : IPermissionGroupPermissionRepository
    {
        private readonly ApplicationDbContext _context;

        public PermissionGroupPermissionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PermissionGroupPermission>> GetGroupPermissionsAsync(List<int> groupIds)
        {
            return await _context.PermissionGroupPermissions
                .Include(pgp => pgp.Permission)
                .Where(pgp =>
                    groupIds.Contains(pgp.PermissionGroupId) &&
                    pgp.Permission.IsActive)
                .ToListAsync();
        }
    }
}
