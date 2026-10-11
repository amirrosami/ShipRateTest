using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Infrastructure.Data;
using System.Security.Claims;
using System.Text;

namespace ShipRate.Application.Services
{
    public class CurrentCompanyService : ICurrentCompanyService
    {
        private const string SessionKey = "CurrentCompanyId";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CurrentCompanyService(
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public int? CompanyId
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    return null;

                var value = httpContext.Session.GetString(SessionKey);

                if (string.IsNullOrWhiteSpace(value))
                    return null;

                if (!int.TryParse(value, out var companyId))
                    return null;

                return companyId;
            }
        }

        public async Task<int?> GetCompanyIdAsync()
        {
            await EnsureCurrentCompanyAsync();

            return CompanyId;
        }

        public async Task<int> GetRequiredCompanyIdAsync()
        {
            await EnsureCurrentCompanyAsync();

            var companyId = CompanyId;

            if (companyId == null)
                throw new InvalidOperationException("Current company is not selected.");

            var hasAccess = await HasAccessAsync(companyId.Value);

            if (!hasAccess)
            {
                Clear();
                throw new UnauthorizedAccessException("User does not have access to the selected company.");
            }

            return companyId.Value;
        }

        public async Task<bool> HasAccessAsync(int companyId)
        {
            var userId = Guid.Parse(GetCurrentUserId());

            if (userId == Guid.Empty)
                return false;

            return await _context.UserCompanies
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.CompanyId == companyId);
        }

        public async Task<bool> HasAccessToCurrentCompanyAsync()
        {
            var companyId = CompanyId;

            if (companyId == null)
                return false;

            return await HasAccessAsync(companyId.Value);
        }

        public async Task SetCurrentCompanyAsync(int companyId)
        {
            var hasAccess = await HasAccessAsync(companyId);

            if (!hasAccess)
                throw new UnauthorizedAccessException("User does not have access to this company.");

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is not available.");

            httpContext.Session.SetString(SessionKey, companyId.ToString());
        }

        public async Task EnsureCurrentCompanyAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return;

            var currentCompanyId = CompanyId;

            if (currentCompanyId != null)
            {
                var hasAccess = await HasAccessAsync(currentCompanyId.Value);

                if (hasAccess)
                    return;

                Clear();
            }

            var firstCompany = await GetFirstAccessibleCompanyAsync();

            if (firstCompany != null)
            {
                httpContext.Session.SetString(SessionKey, firstCompany.Value.ToString());
            }
        }

        public async Task<string?> GetCurrentCompanyNameAsync()
        {
            var companyId = await GetCompanyIdAsync();

            if (companyId == null)
                return null;

            return await _context.UserCompanies
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId.Value)
                .Select(x => x.Company.Person.PersonName)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CurrentCompanyItemDto>> GetAccessibleCompaniesAsync()
        {
            var userId = Guid.Parse(GetCurrentUserId());

            if (userId == Guid.Empty)
                return new List<CurrentCompanyItemDto>();

            return await _context.UserCompanies
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Company.Id)
                .Select(x => new CurrentCompanyItemDto
                {
                    Id = x.CompanyId,
                    Name = x.Company.Person.PersonName
                })
                .ToListAsync();
        }

        public void Clear()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            httpContext?.Session.Remove(SessionKey);
        }

        private async Task<int?> GetFirstAccessibleCompanyAsync()
        {
            var userId = Guid.Parse(GetCurrentUserId());

            if (userId == Guid.Empty)
                return null;

            return await _context.UserCompanies
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Company.Id)
                .Select(x => (int?)x.CompanyId)
                .FirstOrDefaultAsync();
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
        }
    }
}
