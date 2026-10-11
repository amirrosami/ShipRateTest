using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using System.Text.Json;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuditService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task LogAsync(
        string action,
        string entityName,
        string? entityId = null,
        object? oldValues = null,
        object? newValues = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;

        var appUser = user != null
            ? await _userManager.GetUserAsync(user)
            : null;

        var log = new AuditLog
        {
            UserId = appUser.Id,
            UserName = appUser?.UserName,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserAgent = httpContext?.Request.Headers["User-Agent"]
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
