namespace ShipRate.Web.ViewModels.PermissionGroupsManagement
{
    public class UserGroupViewModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
