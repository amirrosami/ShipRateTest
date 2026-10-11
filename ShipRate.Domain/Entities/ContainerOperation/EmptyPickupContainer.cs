using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;
using ShipRate.Domain.Entities.BookingOperation;

namespace ShipRate.Domain.Entities.ContainerOperation
{
    public class EmptyPickupContainer : BusinessEntity
    {
        public Guid EmptyPickupId { get; set; }
        public Guid? RTSInstructionContainerId { get; set; }
        public Guid? RTSInstructionContainerTypeId { get; set; }

        public Guid ContainerId { get; set; }
        public string ContainerNo { get; set; }
        public int ContainerSizeTypeId { get; set; }

        public int ConditionStatusId { get; set; }
        public string? ConditionRemarks { get; set; }
        public string? SealNo { get; set; }
        public string? Remarks { get; set; }

        //Relations
        public Container Container { get; set; }
        public EmptyPickup EmptyPickup { get; set; }
        public RTSInstructionContainer? RTSInstructionContainer { get; set; }
        public RTSInstructionContainerType? RTSInstructionContainerType { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
        public ContainerConditionStatus ConditionStatus { get; set; }

    }
}
