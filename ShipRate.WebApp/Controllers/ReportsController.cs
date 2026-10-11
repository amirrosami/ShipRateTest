using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Reports;
using ShipRate.Application.Interfaces.Manifest;
using ShipRate.Application.Interfaces.Reports;
using System.Data;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IWebHostEnvironment _env;
        private readonly IBLManifestReportService _blManifestReportService;
        private readonly IReportRenderer _reportRenderer;



        public ReportsController(IReportService reportService, IWebHostEnvironment env, IBLManifestReportService blManifestReportService, IReportRenderer reportRenderer)
        {
            _reportService = reportService;
            _env = env;
            _blManifestReportService = blManifestReportService;
            _reportRenderer = reportRenderer;
        }

        //public async Task<IActionResult> Index(int companyId)
        //{
        //    if (companyId <= 0)
        //        return BadRequest("CompanyId is required.");

        //    var model = await _reportService.GetAuthorizedReportsAsync(companyId);
        //    return View(model);
        //}
        public async Task<IActionResult> Index()
        {
            var model = await _reportService.GetAllReportsAsync();
            return View(model);
        }
        public IActionResult Filter(string reportKey)
        {
            if (string.IsNullOrWhiteSpace(reportKey))
                return BadRequest("Report key is required.");

            ViewBag.ReportKey = reportKey;
            return View();
        }

        public async Task<IActionResult> ShowContainersReport()
        {
            var data = await _reportService.GetContainersAsync();
            using Report report = new Report();

            string reportPath = Path.Combine(_env.ContentRootPath, "Reports", "ContainersReport2.frx");
            report.Load(reportPath);

            //report.RegisterData(data, "vwContainers");
            report.RegisterData(data, "vwContainers", 10); // عدد 10 تعداد سطوح (Levels) است که معمولا 1 کافی است
            report.Prepare();

            using MemoryStream ms = new MemoryStream();
            PDFSimpleExport pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, ms);
            return File(ms.ToArray(), "application/pdf");
        }

        public async Task<IActionResult> GetRateRequestReport(Guid id)
        {
            var data = await _reportService.GetRateRequestReportAsync(id);




            var headerList = new List<RateRequestReportDto> { data };















            if (data == null)
                return NotFound();

            using var report = new FastReport.Report();

            var reportPath = Path.Combine(_env.ContentRootPath, "Reports", "RateRequest.frx");
            report.Load(reportPath);



            report.RegisterData(headerList, "RateRequestHeader");
            report.RegisterData(data.PackageTypes ?? new List<RateRequestPackageTypeDto>(), "PackageTypes");
            report.RegisterData(data.ContainerTypes ?? new List<RateRequestContainerTypeDto>(), "ContainerTypes");

            report.GetDataSource("RateRequestHeader").Enabled = true;
            report.GetDataSource("PackageTypes").Enabled = true;
            report.GetDataSource("ContainerTypes").Enabled = true;




            //var ds1 = report.GetDataSource("RateRequestHeader");
            //var ds2 = report.GetDataSource("PackageTypes");
            //var ds3 = report.GetDataSource("ContainerTypes");

            //if (ds1 == null)
            //    throw new Exception("DataSource RateRequestHeader not found in frx.");

            //if (ds2 == null)
            //    throw new Exception("DataSource PackageTypes not found in frx.");

            //if (ds3 == null)
            //    throw new Exception("DataSource ContainerTypes not found in frx.");








            //report.RegisterData(new List<RateRequestReportDto> { data }, "RateRequestHeader");
            //report.RegisterData(data.PackageTypes.ToList(), "PackageTypes");
            //report.RegisterData(data.ContainerTypes.ToList(), "ContainerTypes");
            //report.RegisterData(data.Inquiries, "Inquiries");

            //var allResponses = data.Inquiries
            //    .SelectMany(x => x.Responses)
            //    .ToList();

            //report.RegisterData(allResponses, "InquiryResponses");

            //report.GetDataSource("RateRequestHeader").Enabled = true;
            //report.GetDataSource("PackageTypes").Enabled = true;
            //report.GetDataSource("ContainerTypes").Enabled = true;
            //report.GetDataSource("Inquiries").Enabled = true;
            //report.GetDataSource("InquiryResponses").Enabled = true;

            Console.WriteLine("Rows in Header: " + report.GetDataSource("RateRequestHeader").RowCount);


            report.Prepare();

            //using var stream = new MemoryStream();
            //var pdfExport = new PDFSimpleExport();
            //report.Export(pdfExport, stream);
            //stream.Position = 0;
            //return File(stream.ToArray(), "application/pdf", "RateRequestReport.pdf");


            using MemoryStream ms = new MemoryStream();
            PDFSimpleExport pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, ms);
            return File(ms.ToArray(), "application/pdf");

        }


        public async Task<IActionResult> RateRequestReport(Guid? id)
        {

            var data = new RateRequestReportDto();
            if (id == null)
            {
                data = await _reportService.GetRateRequestReportsAsync();
            }
            else
            {
                data = await _reportService.GetRateRequestReportAsync(id.Value);
            }

            if (data == null)
                return BadRequest("data is null");

            var ds = new DataSet("ReportData");

            // Header table
            var headerTable = new DataTable("RateRequestHeader");
            headerTable.Columns.Add("RequestNumber", typeof(string));
            headerTable.Columns.Add("CustomerName", typeof(string));

            headerTable.Rows.Add(
                data.RequestNumber ?? "",
                data.CustomerName ?? ""
            );

            ds.Tables.Add(headerTable);

            // PackageTypes table
            var packageTable = new DataTable("PackageTypes");
            packageTable.Columns.Add("PackageTypeName", typeof(string));
            packageTable.Columns.Add("UnitCount", typeof(int));

            foreach (var item in data.PackageTypes ?? new List<RateRequestPackageTypeDto>())
            {
                packageTable.Rows.Add(
                    item.PackageTypeName ?? "",
                    item.UnitCount
                );
            }

            ds.Tables.Add(packageTable);

            // ContainerTypes table
            var containerTable = new DataTable("ContainerTypes");
            containerTable.Columns.Add("ContainerSizeTypeName", typeof(string));
            containerTable.Columns.Add("Counts", typeof(decimal));

            foreach (var item in data.ContainerTypes ?? new List<RateRequestContainerTypeDto>())
            {
                containerTable.Rows.Add(
                    item.ContainerSizeTypeName ?? "",
                    item.Counts
                );
            }

            ds.Tables.Add(containerTable);

            Console.WriteLine("C# Header rows: " + headerTable.Rows.Count);
            Console.WriteLine("C# Package rows: " + packageTable.Rows.Count);
            Console.WriteLine("C# Container rows: " + containerTable.Rows.Count);

            using var report = new FastReport.Report();

            var path = Path.Combine(_env.ContentRootPath, "Reports", "RateRequest.frx");
            report.Load(path);

            // خیلی مهم
            report.RegisterData(ds, "ReportData");

            report.GetDataSource("RateRequestHeader").Enabled = true;
            report.GetDataSource("PackageTypes").Enabled = true;
            report.GetDataSource("ContainerTypes").Enabled = true;

            foreach (FastReport.Data.DataSourceBase source in report.Dictionary.DataSources)
            {
                Console.WriteLine(
                    $"DS Name: {source.Name}, Ref: {source.ReferenceName}, Enabled: {source.Enabled}, RowCount: {source.RowCount}"
                );
            }

            report.Prepare();

            using var ms = new MemoryStream();

            var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
            report.Export(pdfExport, ms);

            return File(ms.ToArray(), "application/pdf", "test.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ContainerManifest(
                Guid id,
                CancellationToken cancellationToken)
        {
            var model = await _blManifestReportService.GetAsync(
                id,
                cancellationToken);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> BillOfLading(
            Guid id,
            bool original = false,
            CancellationToken cancellationToken = default)
        {
            var model = await _blManifestReportService.GetBillOfLadingAsync(
                id,
                original,
                cancellationToken);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> ContainerPackingList(Guid id, CancellationToken cancellationToken = default)
        {
            var model = await _blManifestReportService.GetContainerPackingListAsync(
                id,
                cancellationToken);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CargoAllocationByContainer(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var model = await _blManifestReportService
                .GetCargoAllocationByContainerAsync(id, cancellationToken);

            if (model is null)
            {
                return NotFound();
            }

            return View(model);
        }









    }
}
