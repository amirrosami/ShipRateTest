using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IPermissionGroupService
    {
        Task<List<PermissionGroupDto>> GetAllGroupsAsync();
        Task<PermissionGroupDetailDto?> GetGroupDetailAsync(int groupId);
        Task<bool> CreateGroupAsync(CreatePermissionGroupDto dto);
        Task<bool> UpdateGroupAsync(UpdatePermissionGroupDto dto);
        Task<bool> DeleteGroupAsync(int groupId);
        Task<bool> ToggleGroupStatusAsync(int groupId);


        Task<bool> AssignGroupToUserAsync(AssignPermissionGroupToUserDto dto);
        Task<bool> RemoveGroupFromUserAsync(int permissionGroupId, Guid userId);
        Task<List<UserPermissionGroupDto>> GetUserGroupsAsync(Guid userId);
        Task<List<UserPermissionGroupDto>> GetGroupUsersAsync(int permissionGroupId);

    }
}
