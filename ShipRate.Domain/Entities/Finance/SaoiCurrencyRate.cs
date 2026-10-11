namespace ShipRate.Domain.Entities.Finance
{
    public class SaoiCurrencyRate
    {
        public int Id { get; set; }
        public string DateShamsi { get; set; } = "";
        public string DateGregorian { get; set; } = "";
        public decimal ThcEur { get; set; }
        public decimal ThcUsd { get; set; }
        public decimal IranianFloatEur { get; set; }
        public decimal IranianFloatUsd { get; set; }
        public decimal ForeignFloatEur { get; set; }
        public decimal ForeignFloatUsd { get; set; }
        public DateTime FetchedAt { get; set; }
    }
}
