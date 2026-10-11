namespace ShipRate.Application.Interfaces
{
    public interface IPdfExtractionService
    {
        ExtractedDocument ExtractTextUsingPdfPig(string filePath);
    }
    public class ExtractedDocument
    {
        public string RawText { get; set; } = string.Empty;
        public List<string> Words { get; set; } = new();
        public int PageCount { get; set; }

        // فیلدهای نمونه برای فاز اول
        public string? InvoiceNumber { get; set; }
        public string? Date { get; set; }
        public string? TotalAmount { get; set; }
        public string? ProjectNo { get; set; }
        public string? BLNo { get; set; }
    }


}
