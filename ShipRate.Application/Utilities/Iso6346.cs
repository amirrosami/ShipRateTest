namespace ShipRate.Common.Utilities
{
    public class Iso6346
    {
        // جدول تبدیل حروف به عدد طبق استاندارد ISO 6346
        private static readonly Dictionary<char, int> CharValues = new Dictionary<char, int>
    {
        {'A',10},{'B',12},{'C',13},{'D',14},{'E',15},{'F',16},{'G',17},{'H',18},
        {'I',19},{'J',20},{'K',21},{'L',23},{'M',24},{'N',25},{'O',26},{'P',27},
        {'Q',28},{'R',29},{'S',30},{'T',31},{'U',32},{'V',34},{'W',35},{'X',36},
        {'Y',37},{'Z',38}
    };
        public static int CalculateCheckDigit(string containerNumber)
        {
            if (containerNumber.Length != 10)
                throw new ArgumentException("Container number must be 10 characters (Owner+U+Serial)");

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                int value;
                char c = containerNumber[i];
                if (char.IsLetter(c))
                {
                    if (!CharValues.TryGetValue(c, out value))
                        throw new ArgumentException($"Invalid character {c} in container number");
                }
                else if (char.IsDigit(c))
                {
                    value = c - '0';
                }
                else
                {
                    throw new ArgumentException($"Invalid character {c} in container number");
                }

                sum += value * (int)Math.Pow(2, i);
            }

            int checkDigit = sum % 11;
            if (checkDigit == 10) checkDigit = 0;
            return checkDigit;
        }
        // تابع Validator برای شماره کانتینر کامل 11 رقمی
        public static bool ValidateContainerNumber(string fullContainerNumber)
        {
            if (string.IsNullOrEmpty(fullContainerNumber) || fullContainerNumber.Length != 11)
                return false;

            string first10 = fullContainerNumber.Substring(0, 10);
            char lastChar = fullContainerNumber[10];

            // آخرین کاراکتر باید عدد باشد
            if (!char.IsDigit(lastChar))
                return false;

            int expectedCheckDigit = CalculateCheckDigit(first10);
            int actualCheckDigit = lastChar - '0';

            return expectedCheckDigit == actualCheckDigit;
        }
    }
}
