// Domain/Entities/PermissionGroupPermission.cs
using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    /// <summary>
    /// ارتباط بین Group و Permission (Many-to-Many)
    /// </summary>
    public class RolePermission : BusinessEntity
    {
        public Guid RoleId { get; set; }
        public int PermissionId { get; set; }

        //Relations
        public Permission Permission { get; set; } = null!;
        public ApplicationRole Role { get; set; }
    }
}
