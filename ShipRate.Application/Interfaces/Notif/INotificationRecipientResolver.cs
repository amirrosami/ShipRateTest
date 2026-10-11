using ShipRate.Application.DTOs.Notif;
using ShipRate.Domain.Entities.Logs;

namespace ShipRate.Application.Interfaces.Notif
{
    public interface INotificationRecipientResolver
    {
        Task<List<Guid>> ResolveAsync(
            ActivityLog activityLog,
            NotificationRuleDto rule,
            CancellationToken ct = default);
    }
}
