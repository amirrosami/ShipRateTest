using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.BookingOperation
{
    public class BookingContainerTypeCharge : BusinessEntity
    {
        public Guid BookingContainerTypeId { get; set; }
        public int ChargeTypeId { get; set; }
        public decimal ChargeValue { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentTypeId { get; set; }
        public int? PaymentCountryId { get; set; }
        public Guid? PayableToPartyId { get; set; }

        //Relations
        public BookingContainerType? BookingContainerType { get; set; }
        public ChargeType? ChargeType { get; set; }
        public Currency? Currency { get; set; }
        public Country? PaymentAtCountry { get; set; }
        public Person? PayableToParty { get; set; }
    }
}
