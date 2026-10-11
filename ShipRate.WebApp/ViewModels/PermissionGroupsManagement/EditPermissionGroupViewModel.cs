using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.ViewModels.PermissionGroupsManagement
{
    public class EditPermissionGroupViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام گروه الزامی است")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public List<int> SelectedPermissionIds { get; set; } = new();

        public List<PermissionViewModel> AvailablePermissions { get; set; } = new();
    }
}
