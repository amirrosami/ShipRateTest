using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class AddressType : SystemEntity
    {
        public string AddressTypeName { get; set; } = string.Empty;
        public string AddressTypeLocalName { get; set; } = string.Empty;
    }
}
