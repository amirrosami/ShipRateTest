using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BookingOperation;

namespace ShipRate.Domain.Entities.Operations
{
    public class ShipmentBooking : BusinessEntity
    {
        public Guid ShipmentId { get; set; }
        public Guid BookingId { get; set; }

        //Relations
        public Shipment Shipment { get; set; }
        public Booking Booking { get; set; }
    }
}
