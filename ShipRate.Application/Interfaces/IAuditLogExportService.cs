namespace ShipRate.Infrastructure.Services
{
    public interface IAuditLogExportService
    {
        Task<byte[]> ExportToExcelAsync(DateTime? fromDate = null, DateTime? toDate = null, Guid? userId = null, string? action = null);
        Task<byte[]> ExportToPdfAsync(DateTime? fromDate = null, DateTime? toDate = null, Guid? userId = null, string? action = null);
    }
}
