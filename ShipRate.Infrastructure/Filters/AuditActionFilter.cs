using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Json;

namespace ShipRate.Infrastructure.Filters
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditActionFilter(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // ذخیره مقادیر قبلی (برای Update)
            object? oldEntity = null;
            string? entityId = null;
            string? entityName = null;

            // بررسی اگر Action دارای پارامتر id است (برای Update/Delete)
            if (context.ActionArguments.ContainsKey("id"))
            {
                entityId = context.ActionArguments["id"]?.ToString();
                entityName = GetEntityNameFromController(context.Controller.GetType().Name);

                if (!string.IsNullOrEmpty(entityName) && !string.IsNullOrEmpty(entityId))
                {
                    oldEntity = await GetEntitySnapshot(entityName, entityId);
                }
            }

            // اجرای Action
            var executedContext = await next();

            // اگر Action موفق بود، لاگ را ثبت کن
            if (executedContext.Exception == null && executedContext.Result != null)
            {
                var actionName = context.ActionDescriptor.RouteValues["action"];
                var action = DetermineAction(actionName);

                if (!string.IsNullOrEmpty(action))
                {
                    await LogAuditAsync(
                        action,
                        entityName,
                        entityId,
                        oldEntity,
                        context.ActionArguments
                    );
                }
            }
        }

        private async Task<object?> GetEntitySnapshot(string entityName, string entityId)
        {
            try
            {
                return entityName switch
                {
                    "Country" => await _context.Countries
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == int.Parse(entityId)),

                    "City" => await _context.Locations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == Guid.Parse(entityId)),

                    _ => null
                };
            }
            catch
            {
                return null;
            }
        }

        private string? GetEntityNameFromController(string controllerName)
        {
            return controllerName.Replace("Controller", "");
        }

        private string? DetermineAction(string? actionName)
        {
            if (string.IsNullOrEmpty(actionName))
                return null;

            return actionName.ToLower() switch
            {
                "create" => "Create",
                "edit" => "Update",
                "delete" => "Delete",
                "deleteconfirmed" => "Delete",
                _ => null
            };
        }

        private async Task LogAuditAsync(
            string action,
            string? entityName,
            string? entityId,
            object? oldEntity,
            IDictionary<string, object?> actionArguments)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            var userId = Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userName = httpContext.User.Identity?.Name ?? "Anonymous";
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            // مقادیر جدید (از actionArguments)
            object? newEntity = null;
            if (actionArguments.ContainsKey("model"))
            {
                newEntity = actionArguments["model"];
            }
            else if (actionArguments.ContainsKey("country"))
            {
                newEntity = actionArguments["country"];
            }
            else if (actionArguments.ContainsKey("city"))
            {
                newEntity = actionArguments["city"];
            }

            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityName = entityName ?? "Unknown",
                EntityId = entityId,
                OldValues = oldEntity != null ? JsonSerializer.Serialize(oldEntity) : null,
                NewValues = newEntity != null ? JsonSerializer.Serialize(newEntity) : null,
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
    }
}
