using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Reports;
using ShipRate.Common.Report;
//using ShipRate.Application.Interfaces;
//using Stimulsoft.Report;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TestReportsController : Controller
    {

        private readonly ReportService reportService;

        public TestReportsController(ReportService reportService)
        {
            this.reportService = reportService;
        }

        public IActionResult Index()
        {
            return View("", 10);
        }

        public async Task<IActionResult> ViewTestReport(TestReportRequest request)
        {
            request.ReportName = "TestReport";
            var responseview = await reportService.ViewReport<TestReportRequest>(request);
            return View("ViewReport", responseview);
        }







        //        private readonly ICountryReportService _reportService;
        //        public ReportsController(ICountryReportService reportService)
        //        {
        //            _reportService = reportService;
        //        }



        //        public IActionResult Index()
        //        {
        //            return View();
        //        }

        //        public async Task<IActionResult> Country()
        //        {
        //            var data = await _reportService.GetCountriesAsync();

        //            var report = new StiReport();
        //            report.Load("Reports/Countries.mrt"); // فعلاً خالی
        //            report.RegData("Countries", data);
        //            report.Compile();
        //            report.Render();

        //            var stream = new MemoryStream();
        //            report.ExportDocument(StiExportFormat.Pdf, stream);
        //            stream.Position = 0;

        //            return File(stream, "application/pdf", "CountriesReport.pdf");
        //        }
        //    }
    }
}
