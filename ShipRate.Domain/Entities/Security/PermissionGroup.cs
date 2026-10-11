// Domain/Entities/PermissionGroup.cs

namespace ShipRate.Domain.Entities.Security
{
    /// <summary>
    /// گروه‌بندی Permissionها (مثلاً Admin، ReadOnly، Support)
    /// </summary>
    public class PermissionGroup
    {
        public int Id { get; set; }

        /// <summary>
        /// نام یونیک گروه (مثلاً "Admin")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات فارسی
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// آیا فعال است؟
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ICollection<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
            = new List<PermissionGroupPermission>();

        public ICollection<UserPermissionGroup> UserPermissionGroups { get; set; }
            = new List<UserPermissionGroup>();
    }






}
