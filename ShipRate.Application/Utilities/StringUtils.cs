namespace ShipRate.Common.Utilities
{
    public static class StringUtils
    {
        public static string DoubleQuoted(this string input)
        {
            string value = $"""{input}""";
            return value;
        }
    }
}
