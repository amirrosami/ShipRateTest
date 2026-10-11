using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Services.BillOfLadings;
using ShipRate.Application.ViewModels;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;

namespace ShipRate.CustomersPortal.Controllers
{
    [Authorize]
    public class CommitmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ICurrentUserService _currentUser;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();
        private readonly IWebHostEnvironment _environment;
        public CommitmentsController(ApplicationDbContext context ,ICurrentUserService currentUserService, IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
            _currentUser = currentUserService;
        }

        public async Task<IActionResult> Index()
        {
            //var list = new List<CommitmentFileListItemVm>
            //{
            //    new CommitmentFileListItemVm(Guid.NewGuid(),"اصلاحیه کد شناسایی","eslahieCode.docx",DateTime.Now,"اصلاحیه کد شناسایی"),

            //    new CommitmentFileListItemVm(Guid.NewGuid(),"اصلاحیه","eslahie.docx",DateTime.Now,"اصلاحیه"),
            //    new CommitmentFileListItemVm(Guid.NewGuid(),"تعهدنامه مفقودی رسید چک","lafgudicheck.docx" ,DateTime.Now,"تعهدنامه مفقودی رسید چک"),
            //    new CommitmentFileListItemVm(Guid.NewGuid(),"تجمیع بارنامه","tajmi.docx",DateTime.Now,"تجمیع بارنامه"),
            //    new CommitmentFileListItemVm(Guid.NewGuid(),"تفکیک بارنامه" ,"tafkik.docx",DateTime.Now,"تفکیک بارنامه"),
            //    new CommitmentFileListItemVm(Guid.NewGuid(),"درخواست-عودت-سپرده","udatseporde.docx",DateTime.Now,"درخواست-عودت-سپرده "),
            //    new CommitmentFileListItemVm(Guid.NewGuid(), "ایجاد بند ترانزیت", "bandtransit.docx",DateTime.Now ,"ایجاد بند ترانزیت") };

            var list = new List<CommitmentFileListItemVm>
    {
        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "اصلاحیه کد شناسایی",
            "eslahieCode.docx",
            DateTime.Now,
            "اصلاحیه کد شناسایی",
            "Identification Code Amendment",
            "Identification code amendment letter"
        ),

        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "اصلاحیه",
            "eslahie.docx",
            DateTime.Now,
            "اصلاحیه",
            "Amendment",
            "General amendment letter"
        ),

        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "تعهدنامه مفقودی رسید چک",
            "lafgudicheck.docx",
            DateTime.Now,
            "تعهدنامه مفقودی رسید چک",
            "Lost Check Receipt Undertaking",
            "Undertaking letter for lost check receipt"
        ),

        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "تجمیع بارنامه",
            "tajmi.docx",
            DateTime.Now,
            "تجمیع بارنامه",
            "Waybill Consolidation",
            "Waybill consolidation letter"
        ),

        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "تفکیک بارنامه",
            "tafkik.docx",
            DateTime.Now,
            "تفکیک بارنامه",
            "Waybill Separation",
            "Waybill separation letter"
        ),

        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "درخواست-عودت-سپرده",
            "udatseporde.docx",
            DateTime.Now,
            "درخواست-عودت-سپرده",
            "Deposit Refund Request",
            "Request letter for deposit refund"
        ),

        new CommitmentFileListItemVm(
            Guid.NewGuid(),
            "ایجاد بند ترانزیت",
            "bandtransit.docx",
            DateTime.Now,
            "ایجاد بند ترانزیت",
            "Transit Clause Creation",
            "Request for creating transit clause"
        )
    };
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadCommitmentFile(string fileName)
        {
            var fileResult = await LoadCommitmentFileAsync(fileName);
            if (fileResult == null)
                return NotFound();

            return File(fileResult.Value.Content, fileResult.Value.ContentType, fileResult.Value.FileName);
        }

        public async Task<(byte[] Content, string ContentType, string FileName)?> LoadCommitmentFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var safeFileName = Path.GetFileName(fileName);
            var folderPath = Path.Combine(_environment.WebRootPath, "uploads", "commitments");
            var fullPath = Path.Combine(folderPath, safeFileName);

            if (!System.IO.File.Exists(fullPath))
                return null;

            var content = await System.IO.File.ReadAllBytesAsync(fullPath);

            if (!_contentTypeProvider.TryGetContentType(fullPath, out var contentType))
                contentType = "application/octet-stream";

            return (content, contentType, safeFileName);
        }
    }
}
