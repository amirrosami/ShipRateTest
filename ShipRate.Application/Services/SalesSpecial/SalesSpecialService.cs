using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces.SalesSpecial;
using ShipRate.Infrastructure.Data;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace ShipRate.Application.Services.SalesSpecial
{
    public class SalesSpecialService : ISalesSpecialService
    {
        private readonly ApplicationDbContext _db;
        public SalesSpecialService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> GetSalesPrefixAsync(Guid userId)
        {
            return await _db.Users
                   .Where(x => x.Id == userId)
                   .Select(x => x.SalesFilePrefix)
                   .FirstOrDefaultAsync()
                   ?? string.Empty;
        }
        public async Task<int> GetSalesUserNewFileNoAsync(Guid userId)
        {
            if (!_db.Jobs.Where(x => x.SalesUserId == userId).Any()) return 1;
            var count = _db.Jobs.Where(x => x.SalesUserId == userId).Count();
            if (count == 0) return 1;
            var maxNumber = _db.Jobs
                            .AsEnumerable().Where(j => !string.IsNullOrWhiteSpace(j.JobFileNumber) && j.SalesUserId == userId)
                            .Select(j => Regex.Match(j.JobFileNumber, @"\d+").Value)
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(int.Parse)
                            .DefaultIfEmpty(0)
                            .Max();
            return maxNumber + 1;
        }
        public async Task<List<SelectListItem>> GetSalesUsersAsync()
        {
            var salesRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            var salesUsers = await _db.Users
                .Where(u => u.IsActive && _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == salesRoleId))
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FullName
                })
                .ToListAsync();
            return salesUsers;
        }
    }
}
