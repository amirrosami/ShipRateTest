using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Interfaces
{
    public interface IUserPermissionRepository
    {
        // متدهای قبلی...
        Task<UserPermission?> GetUserPermissionAsync(Guid userId, int permissionId);
        Task AddAsync(UserPermission userPermission);
        Task RemoveAsync(UserPermission userPermission);

        // 🆕 متد جدید
        Task<List<UserPermission>> GetUserPermissionsWithDetailsAsync(Guid userId);
    }
}
