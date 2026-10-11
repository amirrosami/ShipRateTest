namespace ShipRate.Common.ExternalServices.Accouting
{
    public class AccDocument
    {
        public DateOnly? PersianDate { get; init; }       // FA_Date
        public DateOnly? GregorianDate { get; init; }     // LA_Date

        // General info
        public string? Description { get; init; }         // Desc
        public string? LatinDescription { get; init; }    // ODesc
        public int? DepartmentCode { get; init; }         // DeptCode

        public DocumentKind Kind { get; init; } = DocumentKind.Normal;
        public DocumentState State { get; init; } = DocumentState.Draft;

        public long? ReferenceNumber { get; init; }       // RefN
        public long? ImportId { get; init; }              // ImpId
        public string? Note { get; init; }

        public string ReferenceId { get; set; }
        public string Status { get; set; }
        public string Dscp { get; set; }

        public List<AccDocumentDetail> AccDocumentDtls { get; set; }
    }
}

