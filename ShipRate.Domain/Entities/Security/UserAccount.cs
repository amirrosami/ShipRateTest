using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Security
{
    public class UserAccount : BusinessEntity
    {
        public Guid PersonId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
