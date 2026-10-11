using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace ShipRate.Infrastructure.HtmlHelpers
{
    public static class AntiForgeryExtensions
    {
        public static string AntiForgeryTokenValue(this IHtmlHelper htmlHelper)
        {
            var antiforgery = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(htmlHelper.ViewContext.HttpContext);
            return tokens.RequestToken ?? string.Empty;
        }
    }
}
