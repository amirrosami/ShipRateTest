using Microsoft.EntityFrameworkCore;
using ShipRate.Common.Notifications.Models;
using ShipRate.Common.Notifications.Repository;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{

    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext db;

        public NotificationRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            db.Notifications.Add(notification);
            await db.SaveChangesAsync();
        }




        public async Task<List<Notification>> GetNewNotifications(int batchSize)
        {
            var notifs = await db.Notifications
                .Where(x => x.Status == NotificationStatus.NotSent || x.Status == NotificationStatus.Failed)
                .Include(x => x.Attachments)
                .Include(x => x.NotificationRecipients)
                .Take(batchSize)
                .ToListAsync();
            return notifs ?? new List<Notification>();
        }

        public async Task UpdateStatus(Guid notificationId, NotificationStatus status)
        {
            var notif = await db.FindAsync<Notification>(notificationId);
            if (notif != null)
            {
                notif.Status = status;
                await db.SaveChangesAsync();
            }
        }
    }
}
