namespace ShipRate.Application.DTOs.Finance
{
    public class ExchangeRateDto
    {
        public Guid Id { get; set; }
        public int ExchangeRateBaseId { get; set; }
        public DateTime ExchangeRateDate { get; set; }
        public string ExchangeRateSource { get; set; } = string.Empty;
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal ExchangeRateValue { get; set; }
        public string ExchangeRatePersianDate { get; set; } = string.Empty;

        public string FromCurrencyName { get; set; } = string.Empty;
        public string ToCurrencyName { get; set; } = string.Empty;
        public string ExchangeRateBaseName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ExchangeRateCreateDto
    {
        public int ExchangeRateBaseId { get; set; }
        public DateTime ExchangeRateDate { get; set; }
        public string ExchangeRateSource { get; set; } = string.Empty;
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal ExchangeRateValue { get; set; }
    }

    public class ExchangeRateUpdateDto
    {
        public Guid Id { get; set; }
        public int ExchangeRateBaseId { get; set; }
        public DateTime ExchangeRateDate { get; set; }
        public string ExchangeRateSource { get; set; } = string.Empty;
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal ExchangeRateValue { get; set; }
    }
}
