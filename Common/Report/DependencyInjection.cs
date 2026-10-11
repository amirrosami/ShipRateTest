using Microsoft.Extensions.DependencyInjection;

namespace ShipRate.Common.Report
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddReportService<TRepo>(this IServiceCollection services)
        where TRepo : class, IReportConfigRepository
        {
            services.AddScoped<IReportConfigRepository, TRepo>();
            services.AddScoped<ReportService>();
            services.AddFastReport();
            return services;
        }
    }
}
