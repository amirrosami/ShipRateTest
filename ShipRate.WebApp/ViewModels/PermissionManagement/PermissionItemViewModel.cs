using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.ViewModels.PermissionManagement
{
    public class PermissionItemViewModel
    {
        public int PermissionId { get; set; }

        [Display(Name = "نام سیستمی")]
        public string Name { get; set; }

        [Display(Name = "نام نمایشی")]
        public string DisplayName { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsAssigned { get; set; }

        // UI Helpers
        public string CheckboxId => $"permission_{PermissionId}";

        public string RowClass => IsAssigned ? "table-primary" : "";

        public string IconClass => IsAssigned ? "bi bi-check-circle-fill text-success" : "bi bi-circle";
    }
}
