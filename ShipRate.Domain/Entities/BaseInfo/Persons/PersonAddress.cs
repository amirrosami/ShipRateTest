using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class PersonAddress : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public int AddressTypeId { get; set; }
        public string AddressTitle { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? CityId { get; set; }
        public string? PostalCode { get; set; }

        //Relations
        public Person Person { get; set; }
        public AddressType AddressType { get; set; }
        public Country Country { get; set; }
        public Location Province { get; set; }
        public Location City { get; set; }
    }
}
