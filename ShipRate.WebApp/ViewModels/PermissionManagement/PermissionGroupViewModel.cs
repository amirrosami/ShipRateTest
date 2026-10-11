using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.ViewModels.PermissionManagement
{
    public class PermissionGroupViewModel
    {
        [Display(Name = "نام گروه")]
        public string GroupName { get; set; }

        [Display(Name = "دسترسی‌ها")]
        public List<PermissionItemViewModel> Permissions { get; set; } = new();

        // Computed Properties
        public int TotalCount => Permissions.Count;

        public int AssignedCount => Permissions.Count(p => p.IsAssigned);

        public bool HasAssignedPermissions => AssignedCount > 0;

        public string BadgeClass =>
            HasAssignedPermissions ? "badge bg-primary" : "badge bg-light text-dark";

        public string StatusText =>
            $"{AssignedCount} از {TotalCount}";
    }
}
