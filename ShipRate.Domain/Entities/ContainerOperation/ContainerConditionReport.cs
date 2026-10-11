using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerConditionReport : BusinessEntity
    {
        public Guid ContainerActionId { get; set; }
        public DateTime InspectionDate { get; private set; }
        public int ContainerConditionStatusId { get; set; }
        public string? InspectorName { get; private set; }
        public string? Remarks { get; private set; }

        //Relation
        public ContainerAction ContainerAction { get; set; }
        public ContainerConditionStatus ContainerConditionStatus { get; set; }
    }
}
