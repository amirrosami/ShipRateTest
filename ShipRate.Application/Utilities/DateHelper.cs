using System.Globalization;
using System.Reflection;

namespace ShipRate.Common.Utilities
{
    public static class DateHelper
    {
        public static string getLatinDateTime8(this string solarDate8)
        {
            string value = $"{solarDate8.Substring(0, 4)}/{solarDate8.Substring(4, 2)}/{solarDate8.Substring(6, 2)}";
            DateTime dt = DateTime.Parse(value, new CultureInfo("fa-IR"));
            return dt.ToString("yyyyMMdd");
        }

        public static DateTime getLatinDateTime(this string solarDate8)
        {
            string value = $"{solarDate8.Substring(0, 4)}/{solarDate8.Substring(4, 2)}/{solarDate8.Substring(6, 2)}";
            DateTime dt = DateTime.Parse(value, new CultureInfo("fa-IR"));
            return dt;
        }
        public static DateTime RemoveTime(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static DateTime? RemoveTime(this DateTime? dateTime)
        {
            return dateTime?.Date;
        }
        public static string getSolarDate10(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(date).ToString() + "/" + pc.GetMonth(date).ToString().PadLeft(2, '0') + "/" + pc.GetDayOfMonth(date).ToString().PadLeft(2, '0');
        }


        public static string getSolarDate8(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(date).ToString() + pc.GetMonth(date).ToString().PadLeft(2, '0') + pc.GetDayOfMonth(date).ToString().PadLeft(2, '0');
        }
        public static DateOnly ToDateOnly(this DateTime date)
        {
            return new DateOnly(date.Year, date.Month, date.Day);
        }

        public static DateOnly? ToDateOnly(this DateTime? date)
        {
            return date != null ? (DateOnly?)date.Value.ToDateOnly() : null;
        }

        public static DateTime ToDateTime(this DateOnly date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }
        public static DateTime? ToDateTime(this DateOnly? date)
        {
            return date != null ? (DateTime?)date.Value.ToDateTime() : null;
        }
        public static CultureInfo GetPersianCulture()
        {
            var culture = new CultureInfo("fa-IR");
            DateTimeFormatInfo formatInfo = culture.DateTimeFormat;
            formatInfo.AbbreviatedDayNames = new[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
            formatInfo.DayNames = new[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهار شنبه", "پنجشنبه", "جمعه", "شنبه" };
            var monthNames = new[]
            {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن",
            "اسفند",
            "",
        };
            formatInfo.AbbreviatedMonthNames =
                formatInfo.MonthNames =
                    formatInfo.MonthGenitiveNames = formatInfo.AbbreviatedMonthGenitiveNames = monthNames;
            formatInfo.AMDesignator = "ق.ظ";
            formatInfo.PMDesignator = "ب.ظ";
            formatInfo.ShortDatePattern = "yyyy/MM/dd";
            formatInfo.LongDatePattern = "dddd, dd MMMM,yyyy";
            formatInfo.FirstDayOfWeek = DayOfWeek.Saturday;
            Calendar cal = new PersianCalendar();
            FieldInfo fieldInfo = culture.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
                fieldInfo.SetValue(culture, cal);
            FieldInfo info = formatInfo.GetType().GetField("calendar", BindingFlags.NonPublic | BindingFlags.Instance);
            if (info != null)
                info.SetValue(formatInfo, cal);
            culture.NumberFormat.NumberDecimalSeparator = "/";
            culture.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
            culture.NumberFormat.NumberNegativePattern = 0;
            return culture;
        }


    }
}
