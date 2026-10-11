using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.ContainerOperation;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class RTSInstructionContainer : BusinessEntity
    {
        public Guid RTSInstructionId { get; set; }
        public Guid ContainerId { get; set; }
        public string ContainerNumber { get; set; }

        //Relations
        public RTSInstruction RTSInstruction { get; set; }
        public Container Container { get; set; }

    }



}
