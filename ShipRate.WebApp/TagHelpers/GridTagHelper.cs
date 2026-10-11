using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;
namespace ShipRate.WebApp.TagHelpers
{
    [HtmlTargetElement("grid")]
    public class GridTagHelper : TagHelper
    {
        public IEnumerable<object> Items { get; set; }
        public Dictionary<string, string> Columns { get; set; }

        public string SortColumn { get; set; }
        public bool SortDesc { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        private static ConcurrentDictionary<string, PropertyInfo[]> _cache = new();

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Items == null || !Items.Any())
            {
                output.Content.SetHtmlContent("<p>No data</p>");
                return;
            }

            // 🔥 SORT
            if (!string.IsNullOrEmpty(SortColumn))
            {
                Items = SortDesc
                    ? Items.OrderByDescending(x => GetNestedValue(x, SortColumn))
                    : Items.OrderBy(x => GetNestedValue(x, SortColumn));
            }

            var total = Items.Count();

            // 🔥 PAGING
            Items = Items
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var sb = new StringBuilder();
            sb.Append("<table class='table table-bordered'>");

            // HEADER
            sb.Append("<thead><tr>");
            foreach (var col in Columns)
            {
                var sortIcon = col.Key == SortColumn
                    ? (SortDesc ? " ↓" : " ↑")
                    : "";

                sb.Append($"<th>{col.Value}{sortIcon}</th>");
            }
            sb.Append("</tr></thead>");

            // BODY
            sb.Append("<tbody>");
            foreach (var item in Items)
            {
                sb.Append("<tr>");
                foreach (var col in Columns)
                {
                    var val = GetNestedValue(item, col.Key) ?? "-";
                    sb.Append($"<td>{val}</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");

            // 🔥 PAGINATION UI
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);
            sb.Append("<div class='mt-2'>");

            for (int i = 1; i <= totalPages; i++)
            {
                if (i == Page)
                    sb.Append($" <strong>{i}</strong> ");
                else
                    sb.Append($" <a href='?page={i}&sortColumn={SortColumn}&sortDesc={SortDesc}'>{i}</a> ");
            }

            sb.Append("</div>");

            output.TagName = "";
            output.Content.SetHtmlContent(sb.ToString());
        }

        // 🔥 Nested property reader
        private object GetNestedValue(object obj, string path)
        {
            if (obj == null) return null;

            var type = obj.GetType();
            var key = $"{type.FullName}.{path}";

            var props = _cache.GetOrAdd(key, _ =>
            {
                var parts = path.Split('.');
                var list = new List<PropertyInfo>();
                var currentType = type;

                foreach (var part in parts)
                {
                    var prop = currentType.GetProperty(part);
                    if (prop == null) return Array.Empty<PropertyInfo>();

                    list.Add(prop);
                    currentType = prop.PropertyType;
                }
                return list.ToArray();
            });

            foreach (var prop in props)
            {
                if (obj == null) return null;
                obj = prop.GetValue(obj);
            }

            return obj;
        }
    }
}