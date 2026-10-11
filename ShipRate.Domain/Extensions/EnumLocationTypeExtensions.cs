using ShipRate.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Domain.Extensions
{
    public static class EnumLocationTypeExtensions
    {
        public static string GetDisplayName(this EnumLocationType locationType)
        {
            var field = locationType.GetType().GetField(locationType.ToString());
            if (field == null) return locationType.ToString();

            var attribute = field
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return attribute?.Name ?? locationType.ToString();
        }

        /// <summary>
        /// بازگرداندن لیست برای Dropdown در View
        /// </summary>
        public static IEnumerable<(int Value, string Display)> GetSelectList()
        {
            return Enum.GetValues<EnumLocationType>()
                .Select(e => ((int)e, e.GetDisplayName()));
        }
    }
}
