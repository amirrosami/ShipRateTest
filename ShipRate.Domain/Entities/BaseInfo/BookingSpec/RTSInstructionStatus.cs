using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.BookingSpec
{
    public class RTSInstructionStatus : SystemEntity
    {
        public string StatusName { get; set; } = string.Empty;
        public string StatusLocalName { get; set; } = string.Empty;
    }
}
