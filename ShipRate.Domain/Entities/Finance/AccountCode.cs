using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Finance;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class AccountCode : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid? PersonId { get; set; }
        public int? ChargeTypeId { get; set; }
        public string? AccCode { get; set; }
        public string? BuyAccCode { get; set; }
        public string? SellAccCode { get; set; }
        //Relations
        public Company Company { get; set; }
        public Person Person { get; set; }
        public ChargeType ChargeType { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser UpdateUser { get; set; }
    }
}