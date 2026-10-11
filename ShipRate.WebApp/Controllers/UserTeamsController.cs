using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.UserTeams;
using ShipRate.Application.Interfaces;
using System.Security.Claims;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class UserTeamsController : Controller
    {
        private readonly IUserTeamService _service;

        public UserTeamsController(IUserTeamService service)
        {
            _service = service;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public IActionResult Index()
        {
            return View();
        }

        // -------------------------------
        // Teams Grid
        // -------------------------------

        public async Task<IActionResult> TeamsGrid(
            int page = 1,
            int? companyId = null,
            int? teamTypeId = null,
            string? search = null)
        {
            var result = await _service.GetTeamsAsync(
                page,
                10,
                CurrentUserId,
                companyId,
                teamTypeId,
                search);

            var vm = new TeamListViewModel
            {
                Result = result,
                CompanyId = companyId,
                TeamTypeId = teamTypeId,
                Search = search
            };

            return PartialView("_TeamsGrid", vm);
        }

        // -------------------------------
        // Members Grid
        // -------------------------------

        public async Task<IActionResult> MembersGrid(Guid teamId)
        {
            var members = await _service.GetTeamMembersAsync(teamId, CurrentUserId);

            ViewBag.TeamId = teamId;

            return PartialView("_MembersGrid", members);
        }

        // -------------------------------
        // Create Team Modal
        // -------------------------------
        public async Task<IActionResult> CreateTeam()
        {
            ViewBag.Companies = await _service.GetCompaniesAsync(CurrentUserId);
            var model = new CreateUserTeamDto();
            model.TeamTypes = await _service.GetTeamTypesAsync();
            return PartialView("_CreateTeamModal", model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTeam(CreateUserTeamDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _service.GetCompaniesAsync(CurrentUserId);
                return PartialView("_CreateTeamModal", dto);
            }

            await _service.CreateTeamAsync(dto, CurrentUserId);

            Response.Headers["HX-Trigger"] = "teamListChanged";

            return Content("");
        }
        // -------------------------------
        // Edit Team Modal
        // -------------------------------

        public async Task<IActionResult> EditTeam(Guid id)
        {
            var team = await _service.GetTeamAsync(id, CurrentUserId);

            if (team == null)
                return NotFound();

            var companies = await _service.GetCompaniesAsync(CurrentUserId);
            var teamTypes = await _service.GetTeamTypesAsync();
            var managers = await _service.GetManagersAsync(team.CompanyId, CurrentUserId);

            var vm = new EditTeamViewModel
            {
                Id = id,
                Team = team,
                Companies = companies,
                TeamTypes = teamTypes,
                Managers = managers
            };

            return PartialView("_EditTeamModal", vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditTeam(Guid id, EditTeamViewModel vm)
        {
            await _service.UpdateTeamAsync(id, vm.Team, CurrentUserId);

            Response.Headers["HX-Trigger"] = "teamListChanged";

            return Ok();
        }

        // -------------------------------
        // Delete Team
        // -------------------------------

        [HttpPost]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            await _service.DeleteTeamAsync(id, CurrentUserId);

            Response.Headers["HX-Trigger"] = "teamListChanged";

            return Ok();
        }

        // -------------------------------
        // Create Member Modal
        // -------------------------------

        public async Task<IActionResult> CreateMember(Guid teamId)
        {
            var users = await _service.GetAvailableUsersAsync(teamId, CurrentUserId);

            var vm = new CreateMemberViewModel
            {
                TeamId = teamId,
                Member = new CreateUserTeamMemberDto
                {
                    UserTeamId = teamId
                },
                Users = users
            };

            return PartialView("_CreateMemberModal", vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel vm)
        {
            await _service.AddMemberAsync(vm.Member, CurrentUserId);

            var members = await _service.GetTeamMembersAsync(vm.Member.UserTeamId, CurrentUserId);

            ViewBag.TeamId = vm.Member.UserTeamId;

            return PartialView("_MembersGrid", members);
        }

        // -------------------------------
        // Delete Member
        // -------------------------------

        [HttpPost]
        public async Task<IActionResult> DeleteMember(Guid id, Guid teamId)
        {
            await _service.RemoveMemberAsync(id, CurrentUserId);

            var members = await _service.GetTeamMembersAsync(teamId, CurrentUserId);

            ViewBag.TeamId = teamId;

            return PartialView("_MembersGrid", members);
        }

        // -------------------------------
        // Managers dropdown reload
        // -------------------------------

        public async Task<IActionResult> ManagersByCompany(int companyId)
        {
            var managers = await _service.GetManagersAsync(companyId, CurrentUserId);
            return PartialView("_ManagersDropdown", managers);
        }
    }
}
