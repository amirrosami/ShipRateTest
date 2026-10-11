using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BookingSpec;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class BookingChange : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int BookingChangeTypeId { get; set; }
        public Guid NewBookingId { get; set; }
        public Guid OldBookingId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public BookingChangeType BookingChangeType { get; set; }
        public Booking NewBooking { get; set; }
        public Booking OldBooking { get; set; }
    }
}
