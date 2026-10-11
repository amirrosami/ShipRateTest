using Microsoft.Extensions.DependencyInjection;

namespace ShipRate.Common.ExternalServices.Tax
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTaxServices<TRepo>(this IServiceCollection services)
            where TRepo : class, ITaxRepository
        {
            services.AddScoped<ITaxRepository, TRepo>();
            services.AddScoped<ExternalTaxService, MoaddianExternalTaxService>();
            services.AddScoped<TaxService>();
            return services;
        }
    }
}
