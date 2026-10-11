namespace ShipRate.Web.ViewModels.PermissionManagement
{
    public class UserListItemViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int PermissionsCount { get; set; }
        public bool IsActive { get; set; }
    }
}
