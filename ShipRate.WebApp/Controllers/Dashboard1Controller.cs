using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class Dashboard1Controller : Controller
    {
        public IActionResult Index()
        {
            //            var countries = new List<CountryDto>
            //{
            //    new CountryDto { Id = 1,  Code = "IR" },
            //    new CountryDto { Id = 2,  Code = "DE" },
            //    new CountryDto { Id = 3, Code = "TR" }
            //};

            //            using Report report = new Report();
            //            report.Load("Reports/c.frx");
            //            report.RegisterData(countries, "Countries");
            //            report.GetDataSource("Countries").Enabled = true;
            //            report.Prepare();

            //            using MemoryStream ms = new MemoryStream();

            //            using PDFSimpleExport pdfExport = new PDFSimpleExport();

            //            report.Export(pdfExport, ms);

            //            return File(ms.ToArray(), "application/pdf");
            return View();
        }
    }
}
