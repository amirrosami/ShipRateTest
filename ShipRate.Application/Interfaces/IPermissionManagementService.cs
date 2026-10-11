using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IPermissionManagementService
    {
        Task<List<UserListItemDto>> GetAllUsersAsync();
        Task<UserPermissionsDto> GetUserPermissionsAsync(Guid userId);
        Task<int> AssignPermissionsAsync(Guid userId, List<int> permissionIds, Guid actionUserId);
        Task<int> RevokePermissionsAsync(Guid userId, List<int> permissionIds);
        Task<int> RevokeAllPermissionsAsync(Guid userId, string revokedByUserId);
        Task<int> AssignPermissionGroupsAsync(Guid userId, List<int> list, Guid currentUserId);
    }
}
