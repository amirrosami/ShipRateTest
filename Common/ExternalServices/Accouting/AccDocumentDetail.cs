namespace ShipRate.Common.ExternalServices.Accouting
{
    public class AccDocumentDetail
    {
        public string AccountCode { get; init; } = default!;   // AccCode
        public string? Detail1 { get; init; }                  // Det1Code
        public string? Detail2 { get; init; }
        public string? Detail3 { get; init; }
        public string? Detail4 { get; init; }
        public string? Detail5 { get; init; }

        // Amounts
        public decimal Debit { get; init; }                    // Deb
        public decimal Credit { get; init; }                   // Cre

        // Currency
        public int? CurrencyCode { get; init; }                // CurrCode
        public decimal? CurrencyAmount { get; init; }          // CurrAmnt

        // Quantity
        public int? UnitCode { get; init; }                    // UnitCode
        public decimal? UnitAmount { get; init; }              // UnitAmnt

        // Dates
        public DateOnly? DuePersianDate { get; init; }         // FA_DueD
        public DateOnly? DueGregorianDate { get; init; }       // LA_DueD

        public long? DueNumber { get; init; }                  // DueN

        public DateOnly? RefPersianDate { get; init; }         // FA_RefD
        public DateOnly? RefGregorianDate { get; init; }       // LA_RefD
        public long? RefNumber { get; init; }                  // RefN

        public DateOnly? OtherPersianDate { get; init; }       // FA_OtherD
        public DateOnly? OtherGregorianDate { get; init; }     // LA_OtherD
        public long? OtherNumber { get; init; }                // OtherN

        // Descriptions
        public string? Description { get; init; }              // Desc
        public string? LatinDescription { get; init; }
    }
}

