namespace ShipRate.Web.ViewModels.PermissionGroupsManagement
{
    public class AssignGroupToUserViewModel
    {
        public int PermissionGroupId { get; set; }
        public string PermissionGroupName { get; set; } = string.Empty;
        public Guid SelectedUserId { get; set; }

        // لیست کاربران موجود
        public List<UserSelectViewModel> AvailableUsers { get; set; } = new();

        // لیست کاربران فعلی Group
        public List<UserGroupViewModel> CurrentUsers { get; set; } = new();
    }
}
