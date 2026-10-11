namespace ShipRate.Web.ViewModels.PermissionGroupsManagement
{
    public class PermissionGroupListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int PermissionCount { get; set; }
    }
}
