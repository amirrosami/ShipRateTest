namespace ShipRate.Application.Utilities
{
    //اشخص حقوقی =شرکت
    public static class NationalIdValidator
    {
        public static bool IsValidNationalId(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
                return false;

            if (nationalCode.Length != 11)
                return false;

            if (!nationalCode.All(char.IsDigit))
                return false;

            if (nationalCode == "11111111111" ||
                nationalCode == "00000000000" ||
                nationalCode == "22222222222" ||
                nationalCode == "33333333333" ||
                nationalCode == "44444444444" ||
                nationalCode == "55555555555" ||
                nationalCode == "66666666666" ||
                nationalCode == "77777777777" ||
                nationalCode == "88888888888" ||
                nationalCode == "99999999999")
            {
                return false;
            }

            int c = int.Parse(nationalCode.Substring(10, 1));
            int a10 = int.Parse(nationalCode.Substring(9, 1)) + 2;

            int n =
                (int.Parse(nationalCode.Substring(0, 1)) + a10) * 29 +
                (int.Parse(nationalCode.Substring(1, 1)) + a10) * 27 +
                (int.Parse(nationalCode.Substring(2, 1)) + a10) * 23 +
                (int.Parse(nationalCode.Substring(3, 1)) + a10) * 19 +
                (int.Parse(nationalCode.Substring(4, 1)) + a10) * 17 +
                (int.Parse(nationalCode.Substring(5, 1)) + a10) * 29 +
                (int.Parse(nationalCode.Substring(6, 1)) + a10) * 27 +
                (int.Parse(nationalCode.Substring(7, 1)) + a10) * 23 +
                (int.Parse(nationalCode.Substring(8, 1)) + a10) * 19 +
                (int.Parse(nationalCode.Substring(9, 1)) + a10) * 17;

            int r = n % 11;

            if (r == 10)
                r = 0;

            return r == c;
        }
    }
}
