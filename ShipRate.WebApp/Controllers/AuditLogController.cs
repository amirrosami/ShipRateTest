using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using ShipRate.Infrastructure.Services;
using ShipRate.Web.ViewModels;

namespace ShipRate.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogExportService _exportService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuditLogController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
    IAuditLogExportService exportService)
        {
            _context = context;
            _exportService = exportService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(AuditLogIndexViewModel filter)
        {
            // Query
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= filter.FromDate);

            if (filter.ToDate.HasValue)
                query = query.Where(a => a.CreatedAt <= filter.ToDate);

            if (filter.UserId is not null)
                query = query.Where(a => a.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.ActionName))
                query = query.Where(a => a.Action == filter.ActionName);

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(a => new AuditLogViewModel
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserEmail = a.User != null ? a.User.Email : "System",
                    Action = a.Action,
                    EntityName = a.EntityName,
                    IpAddress = a.IpAddress,
                    CreatedAt = a.CreatedAt,
                    UserName = a.UserName,
                })
                .ToListAsync();

            var totalCount = await query.CountAsync();

            // ViewModel
            var model = new AuditLogIndexViewModel
            {
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                UserId = filter.UserId,
                ActionName = filter.ActionName,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                Logs = logs,
                Users = new SelectList(
                    await _userManager.Users.ToListAsync(),
                    "Id",
                    "Email",
                    filter.UserId)
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var log = await _context.AuditLogs
                .Where(x => x.Id == id)
                .Select(x => new AuditLogViewModel
                {
                    Id = x.Id,
                    UserName = x.UserName ?? "System",
                    Action = x.Action,
                    EntityName = x.EntityName,
                    EntityId = x.EntityId,
                    OldValues = x.OldValues,
                    NewValues = x.NewValues,
                    IpAddress = x.IpAddress,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (log == null)
                return NotFound();

            return View(log);
        }


        [HttpGet]
        public async Task<IActionResult> ExportExcel(AuditLogFilterViewModel filter)
        {
            var data = await _exportService.ExportToExcelAsync(
                filter.FromDate,
                filter.ToDate,
                filter.UserId,
                filter.ActionName);

            var fileName = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportPdf(AuditLogFilterViewModel filter)
        {
            var data = await _exportService.ExportToPdfAsync(
                filter.FromDate,
                filter.ToDate,
                filter.UserId,
                filter.ActionName);

            var fileName = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(data, "application/pdf", fileName);
        }


    }
}
