namespace ShipRate.Application.DTOs
{
    public class PermissionGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int PermissionCount { get; set; }
    }

    public class PermissionGroupDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new();
    }

    public class CreatePermissionGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class UpdatePermissionGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class AssignPermissionGroupToUserDto
    {
        public int PermissionGroupId { get; set; }
        public Guid UserId { get; set; }
    }


    public class UserPermissionGroupDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PermissionGroupId { get; set; }
        public string PermissionGroupName { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }


}
