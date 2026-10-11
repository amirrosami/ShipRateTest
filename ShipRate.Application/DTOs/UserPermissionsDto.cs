namespace ShipRate.Application.DTOs
{
    public class UserPermissionsDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<PermissionDto> AllPermissions { get; set; }
        public List<int> AssignedPermissionIds { get; set; }
    }
}
