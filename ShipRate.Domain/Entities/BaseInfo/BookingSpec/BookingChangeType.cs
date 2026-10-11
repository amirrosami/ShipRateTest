using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.BookingSpec
{
    public class BookingChangeType : SystemEntity
    {
        public string BookingChangeTypeName { get; set; } = string.Empty;
    }
}
