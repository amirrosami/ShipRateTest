using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipRate.Application.Services.SMS;

namespace ShipRate.Application.Utilities
{
    public static class SmsServiceCollectionExtensions
    {
        public static IServiceCollection AddSmsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmsGatewayOptions>(configuration.GetSection(SmsGatewayOptions.SectionName));

            var smsOptions = configuration.GetSection(SmsGatewayOptions.SectionName).Get<SmsGatewayOptions>()
                             ?? new SmsGatewayOptions();

            services.AddMemoryCache();
            services.AddTransient<SmsAuthenticationHandler>();

            // اکشن مشترک برای ست کردن دقیق تمام هدرهای موجود در اسکرین‌شات پست‌من
            void ConfigureHeaders(HttpClient client)
            {
                client.BaseAddress = new Uri(smsOptions.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.TryAddWithoutValidation("accept", "*/*");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en,fa;q=0.9,la;q=0.8");
                client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Not)A;Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"");
                client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
            }

            // ۱. کلاینت احراز هویت
            services.AddHttpClient<ISmsAuthService, SmsAuthService>(ConfigureHeaders);

            // ۲. کلاینت گیت‌وی (ارسال و استعلام)
            services.AddHttpClient<ISmsGatewayService, SmsGatewayService>(ConfigureHeaders)
                    .AddHttpMessageHandler<SmsAuthenticationHandler>();

            //            services.AddHostedService<SmsOutboxProcessor>();
            if (smsOptions?.IsEnabled == true)
            {
                services.AddHostedService<SmsOutboxProcessor>();
            }

            return services;
        }
    }
}
