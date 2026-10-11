using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using System.Security.Claims;

namespace ShipRate.Application.Services
{

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId => Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

        public string UserName =>
            User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public async Task<int> NumberOfCompaniesAsync()
        {
            Guid userId = this.UserId;
            return _db.UserCompanies.Count(x => x.UserId == userId && !x.IsDeleted);
        }
        public async Task<int> CurrentSelectedCompanyIdAsync()
        {
            //درست شود
            Guid userId = this.UserId;
            return _db.UserCompanies.First(x => x.UserId == userId).CompanyId;
        }
        public async Task<Guid?> GetPersonIdAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            if (user == null) return Guid.Empty;
            return user.PersonId ?? Guid.Empty;
        }
        public async Task<string?> GetUserFullNameAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            if (user == null) return string.Empty;
            return user.FullName ?? string.Empty;
        }

        public bool IsInRole(string role) =>
               _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;

        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;
        }
        public bool IsManager()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole("Manager") ?? false;
        }
        public bool IsFinanceExpert()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole("Finance Expert") ?? false;
        }

        public bool IsSalesManager()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole("Sales Manager") ?? false;
        }
        public bool IsSalesUser()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole("Sales User") ?? false;
        }

        public bool IsOnlySalesUser()
        {
            return (IsSalesUser() && !IsAdmin() && !IsSalesManager() && !IsManager());
        }
        public bool IsOnlyFinanceExpert()
        {
            return (!IsSalesUser() && !IsAdmin() && !IsSalesManager() && !IsManager() && IsFinanceExpert());
        }

        public async Task<string> EmailAddress()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            if (user == null) return string.Empty;
            return user.Email.IsNullOrEmpty() ? string.Empty : user.Email;
        }
    }
}
