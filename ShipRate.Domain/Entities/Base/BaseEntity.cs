namespace ShipRate.Domain.Entities.Base
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
    public abstract class BusinessEntity : BaseEntity<Guid>
    {
        public DateTime CreatedAt { get; set; }
        public Guid CreateUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? EditUserId { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeleteUserId { get; set; }
    }
    public abstract class SystemEntity : BaseEntity<int>
    {
        public bool IsActive { get; set; } = true;
    }
}
