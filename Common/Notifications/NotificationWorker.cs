using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShipRate.Common.Notifications.Repository;

namespace ShipRate.Common.Notifications
{
    public class NotificationWorker : BackgroundService
    {
        private readonly NotificationWorkerConfig _notificationWorkerConfig;
        private readonly IServiceProvider _serviceProvider;
        public NotificationWorker(IServiceProvider serviceProvider, NotificationWorkerConfig config)
        {
            _serviceProvider = serviceProvider;
            _notificationWorkerConfig = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{DateTime.Now} - Notificationworker - Starting NotificationWorker Service...");
            while (!stoppingToken.IsCancellationRequested)
            {

                using (var scope = _serviceProvider.CreateScope())
                {
                    var notifRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                    var notifService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                    var notifs = await notifRepo.GetNewNotifications(_notificationWorkerConfig.NotifBatchSize);
                    if (notifs.Count > 0)
                    {
                        Console.WriteLine($"{DateTime.Now} - Notificationworker - {notifs.Count} new Notifs found. start sending them...");
                    }
                    foreach (var notif in notifs)
                    {
                        await notifService.SendNotification(notif);
                    }
                    if (notifs.Count > 0)
                    {
                        Console.WriteLine($"{DateTime.Now} - NotificationWorker - {notifs.Count} Notifs sent!");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(_notificationWorkerConfig.IntervalSeconds));
            }
        }
    }
}
