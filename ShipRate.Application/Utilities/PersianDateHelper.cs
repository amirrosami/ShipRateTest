using System.Globalization;

namespace ShipRate.Application.Utilities
{
    public static class PersianDateHelper
    {
        private static readonly PersianCalendar PersianCalendar = new PersianCalendar();

        public static string ToShamsi(DateTime dateTime)
        {
            var year = PersianCalendar.GetYear(dateTime);
            var month = PersianCalendar.GetMonth(dateTime);
            var day = PersianCalendar.GetDayOfMonth(dateTime);

            return $"{year:0000}/{month:00}/{day:00}";
        }

        public static string ToShamsi(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            return ToShamsi(dateTime.Value);
        }

        public static DateTime ToGregorian(string shamsiDate)
        {
            if (string.IsNullOrWhiteSpace(shamsiDate))
                throw new ArgumentException("Shamsi date cannot be null or empty.", nameof(shamsiDate));

            shamsiDate = NormalizeDateSeparator(shamsiDate.Trim());

            var parts = shamsiDate.Split('/');
            if (parts.Length != 3)
                throw new FormatException("Shamsi date format is invalid. Expected format: yyyy/MM/dd");

            if (!int.TryParse(parts[0], out var year))
                throw new FormatException("Shamsi year is invalid.");

            if (!int.TryParse(parts[1], out var month))
                throw new FormatException("Shamsi month is invalid.");

            if (!int.TryParse(parts[2], out var day))
                throw new FormatException("Shamsi day is invalid.");

            if (!IsValidShamsiDate(year, month, day))
                throw new FormatException("Shamsi date is not valid.");

            return PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        public static DateTime? ToGregorianNullable(string? shamsiDate)
        {
            if (string.IsNullOrWhiteSpace(shamsiDate))
                return null;

            try
            {
                return ToGregorian(shamsiDate);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValidShamsiDate(string? shamsiDate)
        {
            if (string.IsNullOrWhiteSpace(shamsiDate))
                return false;

            shamsiDate = NormalizeDateSeparator(shamsiDate.Trim());

            var parts = shamsiDate.Split('/');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0], out var year))
                return false;

            if (!int.TryParse(parts[1], out var month))
                return false;

            if (!int.TryParse(parts[2], out var day))
                return false;

            return IsValidShamsiDate(year, month, day);
        }

        public static bool IsValidShamsiDate(int year, int month, int day)
        {
            try
            {
                PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string NormalizeDateSeparator(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Replace("-", "/")
                .Replace(".", "/")
                .Replace("\\", "/");
        }

        public static string NormalizeShamsiDate(string? shamsiDate)
        {
            if (string.IsNullOrWhiteSpace(shamsiDate))
                return string.Empty;

            shamsiDate = NormalizeDateSeparator(shamsiDate.Trim());

            var parts = shamsiDate.Split('/');
            if (parts.Length != 3)
                return shamsiDate;

            if (!int.TryParse(parts[0], out var year))
                return shamsiDate;

            if (!int.TryParse(parts[1], out var month))
                return shamsiDate;

            if (!int.TryParse(parts[2], out var day))
                return shamsiDate;

            return $"{year:0000}/{month:00}/{day:00}";
        }

        public static string GetCurrentShamsiDate()
        {
            return ToShamsi(DateTime.Now);
        }

        public static string GetCurrentShamsiDateTime()
        {
            var now = DateTime.Now;
            var date = ToShamsi(now);
            return $"{date} {now:HH:mm:ss}";
        }

        public static string ToShamsiDateTime(DateTime dateTime)
        {
            var date = ToShamsi(dateTime);
            return $"{date} {dateTime:HH:mm:ss}";
        }

        public static int GetShamsiYear(DateTime dateTime)
        {
            return PersianCalendar.GetYear(dateTime);
        }

        public static int GetShamsiMonth(DateTime dateTime)
        {
            return PersianCalendar.GetMonth(dateTime);
        }

        public static int GetShamsiDay(DateTime dateTime)
        {
            return PersianCalendar.GetDayOfMonth(dateTime);
        }
    }
}
