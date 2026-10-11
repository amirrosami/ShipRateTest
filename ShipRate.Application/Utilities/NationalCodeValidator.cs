namespace ShipRate.Application.Utilities
{
    //اشخص حقیقی=فرد
    public static class NationalCodeValidator
    {
        public static bool IsValidNationalCode(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
                return false;

            if (nationalCode.Length != 10)
                return false;

            if (!nationalCode.All(char.IsDigit))
                return false;

            if (nationalCode.Distinct().Count() == 1)
                return false;

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                int digit = nationalCode[i] - '0';
                sum += digit * (10 - i);
            }

            int checkDigit = nationalCode[9] - '0';
            int remainder = sum % 11;

            return
                (remainder < 2 && checkDigit == remainder) ||
                (remainder >= 2 && checkDigit == 11 - remainder);
        }
    }
}
