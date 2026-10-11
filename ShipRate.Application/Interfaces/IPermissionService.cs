using ShipRate.Application.DTOs;
using System.Security.Claims;

namespace ShipRate.Application.Interfaces
{
    public interface IPermissionService
    {
        /// <summary>
        /// دریافت تمام Permissionهای یک کاربر به‌صورت Claim
        /// </summary>
        Task<List<Claim>> GetUserPermissionClaimsAsync(Guid userId);

        /// <summary>
        /// دریافت نام‌های Permission یک کاربر (برای UI)
        /// </summary>
        Task<List<string>> GetUserPermissionNamesAsync(Guid userId);

        /// <summary>
        /// چک کردن دسترسی خاص
        /// </summary>
        Task<bool> HasPermissionAsync(Guid userId, string permissionName);

        //Task<bool> PermissionExistsAsync(string permissionName);

        Task<List<PermissionDto>> GetAllPermissionsAsync();
        Task<List<PermissionDto>> GetUserPermissionsAsync(Guid userId);
        Task<List<PermissionDto>> GetEffectivePermissionsAsync(Guid userId);



    }
}
