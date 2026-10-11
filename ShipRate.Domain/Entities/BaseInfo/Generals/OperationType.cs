using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class OperationType : SystemEntity
    {
        public string OperationTypeCode { get; set; } = string.Empty;
        public string OperationTypeName { get; set; } = string.Empty;
        public string OperationTypeLocalName { get; set; } = string.Empty;
        public bool IsForContract { get; set; }
        public bool IsForRateRequest { get; set; }
        public bool IsForBooking { get; set; }
        public bool IsForBL { get; set; }
        public bool IsForShipment { get; set; }


        public string? Remarks { get; set; }
    }
}