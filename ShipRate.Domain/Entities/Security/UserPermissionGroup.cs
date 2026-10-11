// Domain/Entities/UserPermissionGroup.cs

namespace ShipRate.Domain.Entities.Security
{
    /// <summary>
    /// Assign کردن Group به User
    /// </summary>
    public class UserPermissionGroup
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int PermissionGroupId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;
        public Guid? AssignedByUserId { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedByUserId { get; set; }



        //Relations
        public PermissionGroup PermissionGroup { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public ApplicationUser AssignedByUser { get; set; } = null!;
    }
}
