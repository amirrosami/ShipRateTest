using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    public class UserCompanyContext : BusinessEntity
    {
        public Guid UserId { get; set; }
        public Guid CompanyContextId { get; set; }
    }
}
