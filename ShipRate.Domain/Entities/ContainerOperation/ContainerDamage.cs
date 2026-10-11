using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerDamage : BusinessEntity
    {
        public Guid? ContainerConditionReportId { get; set; }
        public int ContainerDamageAreaId { get; set; }
        public int ContainerDamageTypeId { get; set; }
        public int ContainerDamageSeverityId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public ContainerConditionReport ContainerConditionReport { get; set; }
        public ContainerDamageArea Area { get; set; }
        public ContainerDamageType DamageType { get; set; }
        public ContainerDamageSeverity Severity { get; set; }
    }
}
