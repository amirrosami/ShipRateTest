using Microsoft.AspNetCore.Identity;

namespace ShipRate.Domain.Entities.Security
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole(string name) : base(name) { }

        //Relations
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}