using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Services
{
    public class AuditLogExportService : IAuditLogExportService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogExportService(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> ExportToExcelAsync(DateTime? fromDate = null, DateTime? toDate = null, Guid? userId = null, string? action = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.CreatedAt <= toDate.Value);

            if (userId is not null)
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Audit Logs");

            // Header styling
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Headers
            worksheet.Cell(1, 1).Value = "Date & Time";
            worksheet.Cell(1, 2).Value = "User";
            worksheet.Cell(1, 3).Value = "Action";
            worksheet.Cell(1, 4).Value = "Entity";
            worksheet.Cell(1, 5).Value = "IP Address";
            worksheet.Cell(1, 6).Value = "Old Values";
            worksheet.Cell(1, 7).Value = "New Values";

            // Data rows
            int row = 2;
            foreach (var log in logs)
            {
                worksheet.Cell(row, 1).Value = log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cell(row, 2).Value = log.User?.Email ?? "System";
                worksheet.Cell(row, 3).Value = log.Action;
                worksheet.Cell(row, 4).Value = log.EntityName;
                worksheet.Cell(row, 5).Value = log.IpAddress ?? "-";
                worksheet.Cell(row, 6).Value = log.OldValues ?? "-";
                worksheet.Cell(row, 7).Value = log.NewValues ?? "-";

                // Action color coding
                var actionCell = worksheet.Cell(row, 3);
                switch (log.Action.ToLower())
                {
                    case "create":
                        actionCell.Style.Font.FontColor = XLColor.Green;
                        break;
                    case "update":
                        actionCell.Style.Font.FontColor = XLColor.Orange;
                        break;
                    case "delete":
                        actionCell.Style.Font.FontColor = XLColor.Red;
                        break;
                }

                row++;
            }

            // Column auto-fit
            worksheet.Columns().AdjustToContents();

            // Freeze header row
            worksheet.SheetView.FreezeRows(1);

            // Add filters
            worksheet.RangeUsed().SetAutoFilter();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportToPdfAsync(DateTime? fromDate = null, DateTime? toDate = null, Guid? userId = null, string? action = null)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.CreatedAt <= toDate.Value);

            if (userId is not null)
                query = query.Where(a => a.UserId == userId);

            if (!string.IsNullOrEmpty(action))
                query = query.Where(a => a.Action == action);

            var logs = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(content => ComposeContent(content, logs));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }


        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Audit Logs Report")
                    .FontSize(20).Bold();

                column.Item().PaddingVertical(5).LineHorizontal(1);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    row.RelativeItem().AlignRight().Text("Country-City Management System");
                });

                column.Item().PaddingVertical(3);
            });
        }

        private void ComposeContent(IContainer container, List<AuditLog> logs)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Date
                    columns.RelativeColumn(2); // User
                    columns.RelativeColumn(1); // Action
                    columns.RelativeColumn(1); // Entity
                    columns.RelativeColumn(1.5f); // IP
                    columns.RelativeColumn(2.5f); // Old Values
                    columns.RelativeColumn(2.5f); // New Values
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Date & Time").Bold();
                    header.Cell().Element(CellStyle).Text("User").Bold();
                    header.Cell().Element(CellStyle).Text("Action").Bold();
                    header.Cell().Element(CellStyle).Text("Entity").Bold();
                    header.Cell().Element(CellStyle).Text("IP Address").Bold();
                    header.Cell().Element(CellStyle).Text("Old Values").Bold();
                    header.Cell().Element(CellStyle).Text("New Values").Bold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .Background(Colors.Grey.Lighten2)
                            .Padding(5)
                            .BorderBottom(1)
                            .BorderColor(Colors.Black);
                    }
                });

                // Data rows
                foreach (var log in logs)
                {
                    table.Cell().Element(DataCellStyle).Text(log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    table.Cell().Element(DataCellStyle).Text(log.User?.Email ?? "System");

                    // Action with color
                    table.Cell().Element(DataCellStyle).Text(text =>
                    {
                        text.Span(log.Action).FontColor(log.Action.ToLower() switch
                        {
                            "create" => Colors.Green.Darken2,
                            "update" => Colors.Orange.Darken1,
                            "delete" => Colors.Red.Darken2,
                            _ => Colors.Black
                        });
                    });

                    table.Cell().Element(DataCellStyle).Text(log.EntityName);
                    table.Cell().Element(DataCellStyle).Text(log.IpAddress ?? "-");
                    table.Cell().Element(DataCellStyle).Text(log.OldValues ?? "-").FontSize(7);
                    table.Cell().Element(DataCellStyle).Text(log.NewValues ?? "-").FontSize(7);
                }

                static IContainer DataCellStyle(IContainer container)
                {
                    return container
                        .Padding(5)
                        .BorderBottom(0.5f)
                        .BorderColor(Colors.Grey.Lighten1);
                }
            });
        }


    }
}
