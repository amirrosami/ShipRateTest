using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Interfaces
{
    public interface IUserPermissionGroupRepository
    {
        Task<UserPermissionGroup?> GetAsync(Guid userId, int permissionGroupId);
        Task<List<UserPermissionGroup>> GetUserGroupsAsync(Guid userId);
        Task<List<UserPermissionGroup>> GetGroupUsersAsync(int permissionGroupId);
        Task AddAsync(UserPermissionGroup userPermissionGroup);
        Task RemoveAsync(UserPermissionGroup userPermissionGroup);
        Task<bool> ExistsAsync(Guid userId, int permissionGroupId);
    }
}
