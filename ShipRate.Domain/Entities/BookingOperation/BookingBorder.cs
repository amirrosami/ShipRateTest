using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class BookingBorder : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public int Sequence { get; set; }
        public Guid BorderId { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public Booking Booking { get; set; }
        public Location Border { get; set; }
    }
}