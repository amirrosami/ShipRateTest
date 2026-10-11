using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace ShipRate.Application.Utilities
{
    public static class HtmlHelperExtensions
    {
        public static string AntiForgeryTokenValue(this IHtmlHelper htmlHelper)
        {
            using var writer = new StringWriter();
            htmlHelper.AntiForgeryToken().WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            var html = writer.ToString();
            var match = Regex.Match(html, "value=\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
