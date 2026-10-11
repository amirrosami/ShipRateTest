using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class PersonExtraInfo : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public int InfoTypeId { get; set; }
        public string PersonExtraInfoValue { get; set; } = string.Empty;
        //Relations
        public Person? Person { get; set; }
        public InfoType? InfoType { get; set; }
    }
}
