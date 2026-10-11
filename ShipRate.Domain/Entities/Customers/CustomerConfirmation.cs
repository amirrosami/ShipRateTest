using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;

namespace ShipRate.Domain.Entities.Customers
{
    public class CustomerConfirmation : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public Guid ConfirmUserId { get; set; }
        public DateTime ConfirmDate { get; set; }
        public string COnfirmRemarks { get; set; } = string.Empty;
        public string Status { get; set; }  //In Progress - Confirmed-Rejected

        //Relations
        public Person Person { get; set; }
        public ApplicationUser ConfirmUser { get; set; }
    }
}
