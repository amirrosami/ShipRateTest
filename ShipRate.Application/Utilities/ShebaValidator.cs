using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
namespace ShipRate.Application.Utilities
{
    public static class ShebaValidator
    {
        public static bool IsValidSheba(string sheba)
        {
            if (string.IsNullOrWhiteSpace(sheba)) return false;
            string cleanSheba = sheba.Replace(" ", "").ToUpper();
            if (!Regex.IsMatch(cleanSheba, "^IR[0-9]{24}$"))
                return false;
            string rearranged = cleanSheba.Substring(4) + cleanSheba.Substring(0, 4);
            StringBuilder numericSheba = new StringBuilder();
            foreach (char c in rearranged)
            {
                if (char.IsDigit(c))
                {
                    numericSheba.Append(c);
                }
                else
                {
                    // تبدیل حرف به عدد: A=10, B=11 و ...
                    numericSheba.Append((c - 'A' + 10).ToString());
                }
            }
            BigInteger bigNumber = BigInteger.Parse(numericSheba.ToString());
            return bigNumber % 97 == 1;
        }
    }
}
