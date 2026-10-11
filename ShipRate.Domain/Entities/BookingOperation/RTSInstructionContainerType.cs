using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class RTSInstructionContainerType : BusinessEntity
    {
        public Guid RTSInstructionId { get; set; }
        public int ContainerSizeTypeId { get; set; }
        public int ContainerCounts { get; set; }
        public string Remarks { get; set; }

        //Relations
        public RTSInstruction RTSInstruction { get; set; }
        public ContainerSizeType ContainerSizeType { get; set; }
    }
}
