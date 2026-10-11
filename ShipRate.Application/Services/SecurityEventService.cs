using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class SecurityEventService : ISecurityEventService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SecurityEventService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogFailedLoginAsync(string email, string reason)
        {
            await LogEventAsync(null, email, "FailedLogin", reason, false);
        }

        public async Task LogSuccessfulLoginAsync(Guid userId)
        {
            await LogEventAsync(userId, null, "SuccessfulLogin", null, true);
        }

        public async Task LogLockoutAsync(Guid userId)
        {
            await LogEventAsync(userId, null, "AccountLockout", "Too many failed attempts", false);
        }

        public async Task LogPasswordChangeAsync(Guid userId)
        {
            await LogEventAsync(userId, null, "PasswordChange", null, true);
        }

        public async Task LogPasswordResetAsync(string email)
        {
            await LogEventAsync(null, email, "PasswordReset", null, true);
        }

        public async Task<int> GetRecentFailedLoginsCountAsync(string email, int minutes = 15)
        {
            var cutoff = DateTime.Now.AddMinutes(-minutes);
            return await _context.SecurityEvents
                .Where(e => e.Email == email
                         && e.EventType == "FailedLogin"
                         && e.CreatedAt >= cutoff)
                .CountAsync();
        }

        private async Task LogEventAsync(
            Guid? userId,
            string? email,
            string eventType,
            string? reason,
            bool isSuccessful)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var securityEvent = new SecurityEvent
            {
                UserId = userId,
                Email = email,
                EventType = eventType,
                Reason = reason,
                IsSuccessful = isSuccessful,
                IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                CreatedAt = DateTime.Now
            };

            _context.SecurityEvents.Add(securityEvent);
            await _context.SaveChangesAsync();
        }
    }
}
