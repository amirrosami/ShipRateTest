using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.ViewModels.PermissionGroupsManagement
{
    public class CreatePermissionGroupViewModel
    {
        [Required(ErrorMessage = "نام گروه الزامی است")]
        [MaxLength(100, ErrorMessage = "نام نباید بیشتر از 100 کاراکتر باشد")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "توضیحات نباید بیشتر از 500 کاراکتر باشد")]
        public string? Description { get; set; }

        public List<int> SelectedPermissionIds { get; set; } = new();

        // برای نمایش در View
        public List<PermissionViewModel> AvailablePermissions { get; set; } = new();
    }
}
