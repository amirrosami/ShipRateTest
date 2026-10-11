using ShipRate.Application.Interfaces;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace ShipRate.Application.Services
{
    public class PdfExtractionService : IPdfExtractionService
    {
        public ExtractedDocument ExtractTextUsingPdfPig(string filePath)
        {
            var result = new ExtractedDocument();
            var sb = new StringBuilder();

            using (var document = PdfDocument.Open(filePath))
            {
                result.PageCount = document.NumberOfPages;

                foreach (var page in document.GetPages())
                {
                    // روش ۱: استخراج متن ساده
                    var text = ContentOrderTextExtractor.GetText(page);
                    sb.AppendLine(text);

                    // روش ۲: اگر نیاز به تحلیل مختصات کلمات دارید (برای فازهای پیشرفته)
                    // var words = page.GetWords(); 
                }
            }
            result.RawText = sb.ToString();
            // استخراج اطلاعات با استفاده از Regex ساده (فاز اول)
            result.InvoiceNumber = TryExtract(result.RawText, @"Invoice\s*(?:No|Number)?[:\s]+([A-Z0-9-]+)");
            result.Date = TryExtract(result.RawText, @"Date[:\s]+(\d{2,4}[/.-]\d{2}[/.-]\d{2,4})");
            result.TotalAmount = TryExtract(result.RawText, @"(?:Total|Amount|Balance)[:\s]+([0-9,.]+)");

            result.BLNo = TryExtract(result.RawText, @"BL\s*(?:No|Number)?[:\s]+([A-Z0-9-]+)");
            result.ProjectNo = TryExtract(result.RawText, @"Project\s*(?:No|Number)?[:\s]+([A-Z0-9-]+)");

            return result;
        }

        private string? TryExtract(string text, string pattern)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }
    }
}
