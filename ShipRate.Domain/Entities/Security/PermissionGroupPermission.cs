// Domain/Entities/PermissionGroupPermission.cs
namespace ShipRate.Domain.Entities.Security
{
    /// <summary>
    /// ارتباط بین Group و Permission (Many-to-Many)
    /// </summary>
    public class PermissionGroupPermission
    {
        public int Id { get; set; }

        public int PermissionGroupId { get; set; }
        public PermissionGroup PermissionGroup { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}
