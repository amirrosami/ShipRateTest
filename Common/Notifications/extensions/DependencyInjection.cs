using Microsoft.Extensions.DependencyInjection;
using ShipRate.Common.Notifications.Repository;

namespace ShipRate.Common.Notifications.extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationServices<TRepo>(this IServiceCollection services, NotificationConfig config)
            where TRepo : class, INotificationRepository
        {
            services.AddSingleton<NotificationConfig>(config);
            services.AddScoped<INotificationRepository, TRepo>();
            services.AddScoped<NotificationService>();
            return services;
        }

        public static IServiceCollection AddNotificationSenderWorker(this IServiceCollection services, NotificationWorkerConfig config)
        {
            services.AddSingleton(config);
            services.AddHostedService<NotificationWorker>();
            return services;
        }
    }
}
