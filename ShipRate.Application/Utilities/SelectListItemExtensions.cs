using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Application.Utilities
{
    public static class SelectListItemExtensions
    {
        public static List<SelectListItem> ToSelectListItems<T>(
            this IEnumerable<T> source,
            Func<T, string> valueSelector,
            Func<T, string> textSelector)
        {
            return source.Select(x => new SelectListItem
            {
                Value = valueSelector(x),
                Text = textSelector(x)
            }).ToList();
        }

    }
}
