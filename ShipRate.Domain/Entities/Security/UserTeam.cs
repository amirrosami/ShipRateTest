using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Security
{
    public class UserTeam : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamLocalName { get; set; } = string.Empty;
        public int TeamTypeId { get; set; }
        public Guid? ManagerId { get; set; }

        //Relations
        public ApplicationUser? Manager { get; set; }
        public TeamType? TeamType { get; set; }
        public Company Company { get; set; }
    }
    public class UserTeamMember : BusinessEntity
    {
        public Guid UserTeamId { get; set; }
        public Guid UserId { get; set; }
        public string? MemberRole { get; set; }
        //Relations
        public UserTeam Team { get; set; }
        public ApplicationUser User { get; set; }
    }
}
