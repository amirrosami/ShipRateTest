using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Hosting;
using ShipRate.Application.Interfaces.Reports;


namespace ShipRate.Application.Services.Reports
{

    public sealed class FastReportRenderer : IReportRenderer
    {
        private readonly IWebHostEnvironment _environment;

        public FastReportRenderer(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<byte[]> ExportPdfAsync<TReportVm>(
            string templateRelativePath,
            TReportVm model,
            Action<Report, TReportVm>? registerData = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var report = await CreatePreparedReportAsync(
                templateRelativePath,
                model,
                registerData,
                cancellationToken);

            using var pdfExport = new PDFSimpleExport();
            using var stream = new MemoryStream();

            report.Export(pdfExport, stream);

            return stream.ToArray();
        }

        public Task<Report> CreatePreparedReportAsync<TReportVm>(
            string templateRelativePath,
            TReportVm model,
            Action<Report, TReportVm>? registerData = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ArgumentException.ThrowIfNullOrWhiteSpace(templateRelativePath);
            ArgumentNullException.ThrowIfNull(model);

            var reportsRootPath = Path.Combine(
                _environment.ContentRootPath,
                "Reports");

            var templatePath = Path.GetFullPath(Path.Combine(
                reportsRootPath,
                templateRelativePath.Replace('/', Path.DirectorySeparatorChar)));

            // جلوگیری از خروج احتمالی از پوشه Reports با ../
            var normalizedReportsRootPath = Path.GetFullPath(reportsRootPath)
                .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;

            if (!templatePath.StartsWith(
                    normalizedReportsRootPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The report template path is outside the Reports directory.");
            }

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    $"FastReport template was not found: {templateRelativePath}",
                    templatePath);
            }

            var report = new Report();

            try
            {
                report.Load(templatePath);

                /*
                 * DataSource پیش‌فرض برای گزارش‌های ساده:
                 *
                 * [Report.BLNo]
                 * [Report.CompanyName]
                 * [Report.Rows.ContainerNumber]
                 */
                report.RegisterData(new[] { model }, "Report");


                EnableDataSource(report, "Report");

                /*
                 * برای گزارش‌های پیچیده، از جمله Container Cargo Manifest،
                 * DataSourceهای اختصاصی توسط فراخواننده ثبت می‌شوند.
                 */
                registerData?.Invoke(report, model);

                report.Prepare();

                return Task.FromResult(report);
            }
            catch
            {
                report.Dispose();
                throw;
            }
        }

        private static void EnableDataSource(
            Report report,
            string dataSourceName)
        {
            var dataSource = report.GetDataSource(dataSourceName);

            if (dataSource is not null)
            {
                dataSource.Enabled = true;
            }
        }

        public static void RegisterAndEnableData<TData>(
            Report report,
            IEnumerable<TData> data,
            string dataSourceName)
        {
            report.RegisterData(data, dataSourceName);
            EnableDataSource(report, dataSourceName);
        }

        public static void RegisterAndEnableSingleData<TData>(
            Report report,
            TData data,
            string dataSourceName)
        {
            report.RegisterData(new[] { data }, dataSourceName);
            EnableDataSource(report, dataSourceName);
        }
    }




}
