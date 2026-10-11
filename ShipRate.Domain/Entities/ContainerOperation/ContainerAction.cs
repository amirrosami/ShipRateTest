using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class ContainerAction : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid ContainerId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public string ContainerNumber { get; set; } = string.Empty;
        public int CntActionTypeId { get; set; }

        public Guid? LocationId { get; set; }
        public Guid? SubLocationId { get; set; }

        public Guid? VoyageId { get; set; }
        public int? VoyageTypeId { get; set; }
        public Guid? LoadListId { get; set; }

        public Guid? RTSInstructionId { get; set; }
        public Guid? BookingId { get; set; }


        public Guid? ContainerVoyageAllocationId { get; set; }
        public int? ContainerConditionStatusId { get; set; }
        public int cntOwnTypeId { get; set; }
        public int FullMTId { get; set; }
        public DateTime ContainerActionDateTime { get; set; }
        public int EventActionSourceId { get; set; }
        public string? ContainerActionSourceReference { get; set; } = string.Empty;

        //Relations

        public Company Company { get; set; }
        public Container Container { get; set; }
        public ContainerUse? ContainerUse { get; set; }
        public CntActionType CntActionType { get; set; }
        public Location Location { get; set; }
        public EventActionSource EventActionSource { get; set; }
        public FullMT FullMT { get; set; }
        public ContainerConditionStatus ContainerCondition { get; set; }

        public Booking? Booking { get; set; }
        public RTSInstruction? RTSInstruction { get; set; }


        public Location SubLocation { get; set; }
        public Voyage Voyage { get; set; }
        public VoyageType VoyageType { get; set; }
        public LoadList LoadList { get; set; }
        public ContainerVoyageAllocation ContainerVoyageAllocation { get; set; }
        public List<ContainerActionAttribute> Attributes { get; set; }
    }


    public class ContainerActionAttribute : BusinessEntity
    {
        public Guid ContainerActionId { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
        public string AttributeValueType { get; set; }
        //Relations
        public ContainerAction ContainerAction { get; set; }
    }
}