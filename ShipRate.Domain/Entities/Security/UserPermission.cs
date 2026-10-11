using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    public class UserPermission : BusinessEntity
    {
        public Guid UserId { get; set; }

        public int PermissionId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;

        public Guid AssignedBy { get; set; } // کدام Admin داده؟

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }

        public virtual Permission Permission { get; set; }
    }
}
