using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    public class UserRole : BusinessEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        ////Relations
        //public ApplicationUser User { get; set; }
        //public ApplicationRole Role { get; set; }
    }
}
