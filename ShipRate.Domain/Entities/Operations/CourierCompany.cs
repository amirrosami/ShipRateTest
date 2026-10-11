using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.Operations
{
    public class CourierCompany : BusinessEntity
    {
        public string Remarks { get; set; }
        //relations
        public Person Person { get; set; }
    }
}
