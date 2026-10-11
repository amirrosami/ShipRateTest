using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Operations
{
    public class EntityReference : BusinessEntity
    {
        public int EntityTypeId { get; set; }
        public Guid EntityId { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceValue { get; set; }
        public bool IsPrimary { get; set; }

        //Relations
        public EntityType EntityType { get; set; }
    }
}
