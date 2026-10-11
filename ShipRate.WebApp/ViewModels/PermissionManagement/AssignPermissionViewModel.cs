using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.ViewModels.PermissionManagement
{
    public class AssignPermissionsViewModel
    {
        [Required(ErrorMessage = "شناسه کاربر الزامی است")]
        public Guid UserId { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
        public string Email { get; set; }



        public List<AssignablePermissionGroupViewModel> UserPermissionGroups { get; set; } = new();

        public List<int> SelectedPermissionGroupIds { get; set; } = new();








        [Display(Name = "گروه‌های دسترسی")]
        public List<PermissionGroupViewModel> PermissionGroups { get; set; } = new();

        [Display(Name = "دسترسی‌های انتخاب شده")]
        public List<int> SelectedPermissionIds { get; set; } = new();

        // ========================================
        // Computed Properties برای UI
        // ========================================

        /// <summary>
        /// تعداد کل دسترسی‌ها
        /// </summary>
        public int TotalPermissions =>
            PermissionGroups?.Sum(g => g.Permissions.Count) ?? 0;

        /// <summary>
        /// تعداد دسترسی‌های فعلی کاربر
        /// </summary>
        public int AssignedPermissions =>
            PermissionGroups?.SelectMany(g => g.Permissions)
                            .Count(p => p.IsAssigned) ?? 0;

        /// <summary>
        /// درصد پیشرفت (برای Progress Bar)
        /// </summary>
        public string ProgressPercentage =>
            TotalPermissions > 0
                ? $"{(AssignedPermissions * 100 / TotalPermissions)}%"
                : "0%";

        /// <summary>
        /// عنوان صفحه
        /// </summary>
        public string PageTitle => $"مدیریت دسترسی‌های {UserName}";

        /// <summary>
        /// آیا کاربر دسترسی دارد؟
        /// </summary>
        public bool HasAnyPermission => AssignedPermissions > 0;

        /// <summary>
        /// CSS Class برای وضعیت
        /// </summary>
        public string StatusBadgeClass =>
            HasAnyPermission ? "badge bg-success" : "badge bg-secondary";

        /// <summary>
        /// متن وضعیت
        /// </summary>
        public string StatusText =>
            HasAnyPermission
                ? $"{AssignedPermissions} دسترسی فعال"
                : "بدون دسترسی";
    }






    public class AssignablePermissionGroupViewModel
    {
        public int PermissionGroupId { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsAssigned { get; set; }

        public string CheckboxId => $"permission_group_{PermissionGroupId}";
    }







}
