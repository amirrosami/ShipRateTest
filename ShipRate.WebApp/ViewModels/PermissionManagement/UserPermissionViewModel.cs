using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.ViewModels.PermissionManagement
{
    public class UserPermissionViewModel
    {
        [Display(Name = "کاربر")]
        public Guid UserId { get; set; }

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "نام")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string? LastName { get; set; }

        [Display(Name = "ایمیل")]
        public string? Email { get; set; }

        public List<PermissionGroupViewModel> PermissionGroups { get; set; } = new();

        // تعداد دسترسی‌های فعال
        public int ActivePermissionsCount =>
            PermissionGroups
                .SelectMany(g => g.Permissions)
                .Count(p => p.IsAssigned);
    }
}
