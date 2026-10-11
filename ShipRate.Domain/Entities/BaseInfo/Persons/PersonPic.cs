using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Persons
{
    public class PersonPic : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public byte[]? Pic { get; set; }
        public byte[]? LocalPic { get; set; }
        //Relations
        public Person? Person { get; set; }
    }
}
