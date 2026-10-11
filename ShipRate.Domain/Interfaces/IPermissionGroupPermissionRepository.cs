using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Interfaces
{
    public interface IPermissionGroupPermissionRepository
    {
        Task<List<PermissionGroupPermission>> GetGroupPermissionsAsync(List<int> groupIds);
    }
}
