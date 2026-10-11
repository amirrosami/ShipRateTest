namespace ShipRate.Application.DTOs
{
    public class GuaranteePlanDto
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string LetterNo { get; set; }
        public DateTime LetterDate { get; set; }
        public int InOutId { get; set; }
        public string InOutName { get; set; }
        public Guid? ShippingLineId { get; set; }
        public string ShippingLineName { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid BLId { get; set; }
        public string BLNo { get; set; }
        public string Remarks { get; set; }
    }

    public class GuaranteePlanCreateOrUpdateDto
    {
        public Guid? Id { get; set; } // Null for Create, Valued for Edit
        public int CompanyId { get; set; }
        public string LetterNo { get; set; }
        public DateTime LetterDate { get; set; }
        public int InOutId { get; set; }
        public Guid? ShippingLineId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid BLId { get; set; }
        public string BLNo { get; set; }
        public string Remarks { get; set; }
    }

    public class GuaranteePlanSearchDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
    }

    public class BlLookupResultDto
    {
        public Guid BLId { get; set; }
        public string BLNo { get; set; }
    }
}
