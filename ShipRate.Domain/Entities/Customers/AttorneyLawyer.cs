using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Customers
{
    public class AttorneyLawyer : BusinessEntity
    {
        public Guid AttorneyId { get; set; }
        public Guid LawyerId { get; set; }
        //Relations

        public Attorney Attorney { get; set; }
        public Person Lawyer { get; set; }
    }
}
