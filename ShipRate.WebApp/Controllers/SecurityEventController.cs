using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using ShipRate.Web.ViewModels;

namespace ShipRate.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SecurityEventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SecurityEventController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(SecurityEventFilterViewModel filter)
        {
            var query = _context.SecurityEvents
                .Include(e => e.User)
                .AsQueryable();

            // Filters
            if (filter.FromDate.HasValue)
                query = query.Where(e => e.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(e => e.CreatedAt <= filter.ToDate.Value.AddDays(1));

            if (filter.UserId is not null)
                query = query.Where(e => e.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(e => e.Email!.Contains(filter.Email));

            if (!string.IsNullOrEmpty(filter.EventType))
                query = query.Where(e => e.EventType == filter.EventType);

            if (filter.IsSuccessful.HasValue)
                query = query.Where(e => e.IsSuccessful == filter.IsSuccessful.Value);

            // Statistics
            var totalFailedLogins = await _context.SecurityEvents
                .CountAsync(e => e.EventType == "FailedLogin");
            var totalLockouts = await _context.SecurityEvents
                .CountAsync(e => e.EventType == "AccountLockout");
            var totalPasswordChanges = await _context.SecurityEvents
                .CountAsync(e => e.EventType == "PasswordChange");

            // Count
            var totalCount = await query.CountAsync();

            // Get data
            var events = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(e => new SecurityEventViewModel
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    UserEmail = e.User != null ? e.User.Email : null,
                    AttemptedEmail = e.Email,
                    EventType = e.EventType,
                    Reason = e.Reason,
                    IpAddress = e.IpAddress,
                    UserAgent = e.UserAgent,
                    IsSuccessful = e.IsSuccessful,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            // Build ViewModel
            var model = new SecurityEventIndexViewModel
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                UserId = filter.UserId,
                Email = filter.Email,
                EventType = filter.EventType,
                IsSuccessful = filter.IsSuccessful,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                TotalRecords = totalCount,
                Events = events,
                Users = new SelectList(
                    await _userManager.Users.ToListAsync(),
                    "Id",
                    "Email",
                    filter.UserId),
                TotalFailedLogins = totalFailedLogins,
                TotalLockouts = totalLockouts,
                TotalPasswordChanges = totalPasswordChanges
            };

            return View(model);
        }
    }
}
