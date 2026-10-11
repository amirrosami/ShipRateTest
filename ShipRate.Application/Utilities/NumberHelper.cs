namespace ShipRate.Common.Utilities
{
    public static class NumberHelper
    {
        public static string GetEnglishNumber(this string persianNumber)
        {
            string englishNumber = "";
            foreach (char ch in persianNumber)
            {
                englishNumber += char.GetNumericValue(ch);
            }
            return englishNumber;
        }
        public static string GetPersianNumber(this string englishNumber)
        {
            string persianNumber = "";
            foreach (char ch in englishNumber)
            {
                persianNumber += (char)(1776 + char.GetNumericValue(ch));
            }
            return persianNumber;
        }
    }
}
