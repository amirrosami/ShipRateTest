using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Application.DTOs.UserTeams
{
    public class UserTeamDto
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string TeamName { get; set; }
        public string TeamLocalName { get; set; }
        public int TeamTypeId { get; set; }
        public string TeamTypeName { get; set; }
        public Guid? ManagerId { get; set; }
        public string ManagerName { get; set; }
    }

    public class UserTeamMemberDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string MemberRole { get; set; }
    }

    public class CreateUserTeamDto
    {
        public int CompanyId { get; set; }

        public string TeamName { get; set; }

        public string TeamLocalName { get; set; }

        public int TeamTypeId { get; set; }

        public Guid? ManagerId { get; set; }
        public List<SelectListItem>? TeamTypes { get; set; }
    }

    public class CreateUserTeamMemberDto
    {
        public Guid UserTeamId { get; set; }

        public Guid UserId { get; set; }

        public string MemberRole { get; set; }
    }

    public class TeamListViewModel
    {
        public PagedResultDto<UserTeamDto> Result { get; set; }

        public int? CompanyId { get; set; }

        public int? TeamTypeId { get; set; }

        public string? Search { get; set; }
    }

    public class CreateTeamViewModel
    {
        public CreateUserTeamDto Team { get; set; }

        public List<SelectListItem> Companies { get; set; }

        public List<SelectListItem> TeamTypes { get; set; }

        public List<SelectListItem> Managers { get; set; }
    }

    public class EditTeamViewModel
    {
        public Guid Id { get; set; }

        public CreateUserTeamDto Team { get; set; }

        public List<SelectListItem> Companies { get; set; }

        public List<SelectListItem> TeamTypes { get; set; }

        public List<SelectListItem> Managers { get; set; }
    }
    public class CreateMemberViewModel
    {
        public Guid TeamId { get; set; }

        public CreateUserTeamMemberDto Member { get; set; }

        public List<SelectListItem> Users { get; set; }
    }
}
