using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.UserTeams;

namespace ShipRate.Application.Interfaces
{
    public interface IUserTeamService
    {
        Task<PagedResultDto<UserTeamDto>> GetTeamsAsync(
            int page,
            int pageSize,
            Guid currentUserId,
            int? companyId = null,
            int? teamTypeId = null,
            string? search = null);

        Task<List<UserTeamMemberDto>> GetTeamMembersAsync(Guid teamId, Guid currentUserId);

        Task<CreateUserTeamDto?> GetTeamAsync(Guid id, Guid currentUserId);

        Task CreateTeamAsync(CreateUserTeamDto input, Guid currentUserId);

        Task UpdateTeamAsync(Guid id, CreateUserTeamDto input, Guid currentUserId);

        Task DeleteTeamAsync(Guid id, Guid currentUserId);

        Task AddMemberAsync(CreateUserTeamMemberDto input, Guid currentUserId);

        Task RemoveMemberAsync(Guid memberId, Guid currentUserId);

        Task<List<SelectListItem>> GetCompaniesAsync(Guid currentUserId);

        Task<List<SelectListItem>> GetTeamTypesAsync();

        Task<List<SelectListItem>> GetManagersAsync(int companyId, Guid currentUserId);

        Task<List<SelectListItem>> GetAvailableUsersAsync(Guid teamId, Guid currentUserId);
    }
}
