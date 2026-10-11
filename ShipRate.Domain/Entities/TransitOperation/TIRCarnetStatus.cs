using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class TIRCarnetStatus : SystemEntity
    {
        public string TIRCarnetStatusTitle { get; set; } = string.Empty;
    }
}

//Status NVARCHAR(20) -- New / Active / Suspended / Closed

