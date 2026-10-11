using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.UserTeams;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class UserTeamService : IUserTeamService
    {
        private readonly ApplicationDbContext _context;

        public UserTeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Teams

        public async Task<PagedResultDto<UserTeamDto>> GetTeamsAsync(
            int page,
            int pageSize,
            Guid currentUserId,
            int? companyId = null,
            int? teamTypeId = null,
            string? search = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var accessibleCompanyIds = await GetAccessibleCompanyIdsAsync(currentUserId);

            var query =
                from t in _context.UserTeams.AsNoTracking()
                join c in _context.Companies.AsNoTracking() on t.CompanyId equals c.Id
                join p in _context.Persons.AsNoTracking() on c.PersonId equals p.Id
                join tt in _context.TeamTypes.AsNoTracking() on t.TeamTypeId equals tt.Id
                join mgr in _context.Users.AsNoTracking() on t.ManagerId equals mgr.Id into mgrJoin
                from manager in mgrJoin.DefaultIfEmpty()
                where !t.IsDeleted
                      && accessibleCompanyIds.Contains(t.CompanyId)
                select new UserTeamDto
                {
                    Id = t.Id,
                    CompanyId = t.CompanyId,
                    CompanyName = p.PersonName,
                    TeamName = t.TeamName,
                    TeamLocalName = t.TeamLocalName,
                    TeamTypeId = t.TeamTypeId,
                    TeamTypeName = tt.TeamTypeName,
                    ManagerId = t.ManagerId,
                    ManagerName = manager != null ? manager.FullName : null
                };

            if (companyId.HasValue)
                query = query.Where(x => x.CompanyId == companyId.Value);

            if (teamTypeId.HasValue)
                query = query.Where(x => x.TeamTypeId == teamTypeId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(x =>
                    x.TeamName.Contains(search) ||
                    x.TeamLocalName.Contains(search) ||
                    x.CompanyName.Contains(search) ||
                    x.TeamTypeName.Contains(search) ||
                    (x.ManagerName != null && x.ManagerName.Contains(search)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.CompanyName)
                .ThenBy(x => x.TeamTypeName)
                .ThenBy(x => x.TeamName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<UserTeamDto>(items, totalCount, page, pageSize);
        }

        public async Task<CreateUserTeamDto?> GetTeamAsync(Guid id, Guid currentUserId)
        {
            var team = await _context.UserTeams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (team == null)
                return null;

            await EnsureUserHasCompanyAccessAsync(team.CompanyId, currentUserId);

            return new CreateUserTeamDto
            {
                CompanyId = team.CompanyId,
                TeamName = team.TeamName,
                TeamLocalName = team.TeamLocalName,
                TeamTypeId = team.TeamTypeId,
                ManagerId = team.ManagerId
            };
        }

        public async Task CreateTeamAsync(CreateUserTeamDto input, Guid currentUserId)
        {
            await EnsureUserHasCompanyAccessAsync(input.CompanyId, currentUserId);
            await ValidateTeamInputAsync(input, ignoreTeamId: null);

            var entity = new UserTeam
            {
                Id = Guid.CreateVersion7(),
                CompanyId = input.CompanyId,
                TeamName = input.TeamName.Trim(),
                TeamLocalName = input.TeamLocalName?.Trim(),
                TeamTypeId = input.TeamTypeId,
                ManagerId = input.ManagerId,
                //IsActive = true,
                IsDeleted = false,

                // در صورت وجود این فیلدها در جدول:
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            _context.UserTeams.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTeamAsync(Guid id, CreateUserTeamDto input, Guid currentUserId)
        {
            var entity = await _context.UserTeams
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Team not found.");

            await EnsureUserHasCompanyAccessAsync(entity.CompanyId, currentUserId);
            await EnsureUserHasCompanyAccessAsync(input.CompanyId, currentUserId);

            await ValidateTeamInputAsync(input, ignoreTeamId: id);

            entity.CompanyId = input.CompanyId;
            entity.TeamName = input.TeamName.Trim();
            entity.TeamLocalName = input.TeamLocalName?.Trim();
            entity.TeamTypeId = input.TeamTypeId;
            entity.ManagerId = input.ManagerId;

            // در صورت وجود این فیلدها:
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamAsync(Guid id, Guid currentUserId)
        {
            var team = await _context.UserTeams
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (team == null)
                throw new Exception("Team not found.");

            await EnsureUserHasCompanyAccessAsync(team.CompanyId, currentUserId);

            team.IsDeleted = true;
            //team.IsActive = false;

            // اگر soft delete اعضا هم می‌خواهی:
            var members = await _context.UserTeamMembers
                .Where(x => x.UserTeamId == id && !x.IsDeleted)
                .ToListAsync();

            foreach (var member in members)
            {
                member.IsDeleted = true;
                //member.IsActive = false;
                member.UpdatedAt = DateTime.Now;
                member.EditUserId = currentUserId;
            }

            team.UpdatedAt = DateTime.Now;
            team.EditUserId = currentUserId;

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Members

        public async Task<List<UserTeamMemberDto>> GetTeamMembersAsync(Guid teamId, Guid currentUserId)
        {
            var team = await _context.UserTeams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == teamId && !x.IsDeleted);

            if (team == null)
                return new List<UserTeamMemberDto>();

            await EnsureUserHasCompanyAccessAsync(team.CompanyId, currentUserId);

            var items = await
                (from m in _context.UserTeamMembers.AsNoTracking()
                 join u in _context.Users.AsNoTracking() on m.UserId equals u.Id
                 where m.UserTeamId == teamId && !m.IsDeleted
                 orderby u.FullName
                 select new UserTeamMemberDto
                 {
                     Id = m.Id,
                     UserId = m.UserId,
                     FullName = u.FullName,
                     MemberRole = m.MemberRole
                 })
                .ToListAsync();

            return items;
        }

        public async Task AddMemberAsync(CreateUserTeamMemberDto input, Guid currentUserId)
        {
            var team = await _context.UserTeams
                .FirstOrDefaultAsync(x => x.Id == input.UserTeamId && !x.IsDeleted);

            if (team == null)
                throw new Exception("Team not found.");

            await EnsureUserHasCompanyAccessAsync(team.CompanyId, currentUserId);

            var userAccessibleInCompany = await _context.UserCompanies
                .AnyAsync(x =>
                    x.UserId == input.UserId &&
                    x.CompanyId == team.CompanyId &&
                    //x.IsActive &&
                    !x.IsDeleted);

            if (!userAccessibleInCompany)
                throw new Exception("Selected user does not belong to this company.");

            var alreadyInThisTeam = await _context.UserTeamMembers
                .AnyAsync(x =>
                    x.UserTeamId == input.UserTeamId &&
                    x.UserId == input.UserId &&
                    !x.IsDeleted);

            if (alreadyInThisTeam)
                throw new Exception("This user is already a member of the selected team.");

            // محدودیت: حداکثر 2 تیم با TeamType یکسان در همان شرکت
            var sameTypeTeamCount = await
                (from m in _context.UserTeamMembers
                 join t in _context.UserTeams on m.UserTeamId equals t.Id
                 where m.UserId == input.UserId
                       && !m.IsDeleted
                       && !t.IsDeleted
                       && t.CompanyId == team.CompanyId
                       && t.TeamTypeId == team.TeamTypeId
                 select m.Id)
                .CountAsync();

            if (sameTypeTeamCount >= 2)
                throw new Exception("User cannot belong to more than two teams of the same type in this company.");

            var entity = new UserTeamMember
            {
                Id = Guid.CreateVersion7(),
                UserTeamId = input.UserTeamId,
                UserId = input.UserId,
                MemberRole = input.MemberRole?.Trim(),
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = currentUserId
            };

            _context.UserTeamMembers.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(Guid memberId, Guid currentUserId)
        {
            var member = await _context.UserTeamMembers
                .FirstOrDefaultAsync(x => x.Id == memberId && !x.IsDeleted);

            if (member == null)
                throw new Exception("Team member not found.");

            var team = await _context.UserTeams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == member.UserTeamId && !x.IsDeleted);

            if (team == null)
                throw new Exception("Parent team not found.");

            await EnsureUserHasCompanyAccessAsync(team.CompanyId, currentUserId);

            member.IsDeleted = true;
            //member.IsActive = false;
            member.UpdatedAt = DateTime.Now;
            member.EditUserId = currentUserId;

            await _context.SaveChangesAsync();
        }

        #endregion

        #region Dropdowns

        public async Task<List<SelectListItem>> GetCompaniesAsync(Guid currentUserId)
        {
            var items = await
                (from uc in _context.UserCompanies.AsNoTracking()
                 join c in _context.Companies.AsNoTracking() on uc.CompanyId equals c.Id
                 join p in _context.Persons.AsNoTracking() on c.PersonId equals p.Id
                 where uc.UserId == currentUserId
                       //&& uc.IsActive
                       && !uc.IsDeleted
                       && c.IsActive
                       && !c.IsDeleted
                 orderby c.Id
                 select new SelectListItem
                 {
                     Value = c.Id.ToString(),
                     Text = p.PersonName
                 })
                .Distinct()
                .ToListAsync();

            return items;
        }

        public async Task<List<SelectListItem>> GetTeamTypesAsync()
        {
            return await _context.TeamTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TeamTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetManagersAsync(int companyId, Guid currentUserId)
        {
            await EnsureUserHasCompanyAccessAsync(companyId, currentUserId);

            var users = await
                (from uc in _context.UserCompanies.AsNoTracking()
                 join u in _context.Users.AsNoTracking() on uc.UserId equals u.Id
                 where uc.CompanyId == companyId
                       //&& uc.IsActive
                       && !uc.IsDeleted
                       && u.IsActive
                 orderby u.FullName
                 select new SelectListItem
                 {
                     Value = u.Id.ToString(),
                     Text = u.FullName
                 })
                .Distinct()
                .ToListAsync();

            return users;
        }

        public async Task<List<SelectListItem>> GetAvailableUsersAsync(Guid teamId, Guid currentUserId)
        {
            var team = await _context.UserTeams
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == teamId && !x.IsDeleted);

            if (team == null)
                return new List<SelectListItem>();

            await EnsureUserHasCompanyAccessAsync(team.CompanyId, currentUserId);

            var usersAlreadyInTeam = _context.UserTeamMembers
                .AsNoTracking()
                .Where(x => x.UserTeamId == teamId && !x.IsDeleted)
                .Select(x => x.UserId);

            var availableUsers = await
                (from uc in _context.UserCompanies.AsNoTracking()
                 join u in _context.Users.AsNoTracking() on uc.UserId equals u.Id
                 where uc.CompanyId == team.CompanyId
                       //&& uc.IsActive
                       && !uc.IsDeleted
                       && u.IsActive
                       && !usersAlreadyInTeam.Contains(u.Id)
                 orderby u.FullName
                 select new SelectListItem
                 {
                     Value = u.Id.ToString(),
                     Text = u.FullName
                 })
                .Distinct()
                .ToListAsync();

            return availableUsers;
        }

        #endregion

        #region Helpers

        private async Task<List<int>> GetAccessibleCompanyIdsAsync(Guid currentUserId)
        {
            return await _context.UserCompanies
                .AsNoTracking()
                .Where(x => x.UserId == currentUserId && !x.IsDeleted)
                .Select(x => x.CompanyId)
                .Distinct()
                .ToListAsync();
        }

        private async Task EnsureUserHasCompanyAccessAsync(int companyId, Guid currentUserId)
        {
            var hasAccess = await _context.UserCompanies
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == currentUserId &&
                    x.CompanyId == companyId &&
                    //x.IsActive &&
                    !x.IsDeleted);

            if (!hasAccess)
                throw new Exception("You do not have access to this company.");
        }

        private async Task ValidateTeamInputAsync(CreateUserTeamDto input, Guid? ignoreTeamId)
        {
            if (input == null)
                throw new Exception("Input is required.");

            if (input.CompanyId <= 0)
                throw new Exception("Company is required.");

            if (string.IsNullOrWhiteSpace(input.TeamName))
                throw new Exception("Team name is required.");

            if (input.TeamTypeId <= 0)
                throw new Exception("Team type is required.");

            var teamTypeExists = await _context.TeamTypes
                .AsNoTracking()
                .AnyAsync(x => x.Id == input.TeamTypeId && !x.IsDeleted);

            if (!teamTypeExists)
                throw new Exception("Selected team type is invalid.");

            if (input.ManagerId.HasValue)
            {
                var managerInCompany = await _context.UserCompanies
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.UserId == input.ManagerId.Value &&
                        x.CompanyId == input.CompanyId &&
                        //x.IsActive &&
                        !x.IsDeleted);

                if (!managerInCompany)
                    throw new Exception("Selected manager does not belong to the selected company.");
            }

            var duplicateQuery = _context.UserTeams.AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == input.CompanyId &&
                    x.TeamTypeId == input.TeamTypeId &&
                    x.TeamName == input.TeamName.Trim());

            if (ignoreTeamId.HasValue)
                duplicateQuery = duplicateQuery.Where(x => x.Id != ignoreTeamId.Value);

            var duplicateExists = await duplicateQuery.AnyAsync();

            if (duplicateExists)
                throw new Exception("A team with the same name and type already exists in this company.");
        }

        #endregion
    }
}