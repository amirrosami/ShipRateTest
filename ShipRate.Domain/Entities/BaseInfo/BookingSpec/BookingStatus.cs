using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.BookingSpec
{
    public class BookingStatus : SystemEntity
    {
        public string BookingStatusCode { get; set; }
        public string BookingStatusName { get; set; }
        public string BookingStatusLocalName { get; set; }
    }
}
