using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs.Documents;
using ShipRate.Application.Interfaces.Documents;
using ShipRate.WebApp.ViewModels.Documents;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(IDocumentService documentService, IWebHostEnvironment env)
        {
            _documentService = documentService;
            _env = env;
        }
        [Authorize("Utilities.Documents.View")]
        public async Task<IActionResult> Index()
        {
            var vm = new DocumentIndexViewModel
            {
                Search = new DocumentSearchDto
                {
                    PageNumber = 1,
                    PageSize = 10
                },
                Companies = await _documentService.GetCompanySelectListAsync(),
                DocumentTypes = await _documentService.GetDocumentTypeSelectListAsync(),
                ObjectTypes = await _documentService.GetObjectTypeSelectListAsync(),
                IsActiveList = _documentService.GetIsActiveSelectList(),
                PageSizeList = GetPageSizeList()
            };

            vm.Result = await _documentService.SearchAsync(vm.Search);

            return View(vm);
        }

        [HttpGet]
        [Authorize("Utilities.Documents.View")]
        public async Task<IActionResult> Search(DocumentSearchDto search)
        {
            if (search.PageNumber <= 0)
                search.PageNumber = 1;

            if (search.PageSize <= 0)
                search.PageSize = 10;

            var vm = new DocumentIndexViewModel
            {
                Search = search,
                Result = await _documentService.SearchAsync(search),
                Companies = await _documentService.GetCompanySelectListAsync(),
                DocumentTypes = await _documentService.GetDocumentTypeSelectListAsync(),
                ObjectTypes = await _documentService.GetObjectTypeSelectListAsync(),
                IsActiveList = _documentService.GetIsActiveSelectList(),
                PageSizeList = GetPageSizeList()
            };

            return PartialView("_DocumentListPartial", vm);
        }

        [HttpGet]
        [Authorize("Utilities.Documents.View")]
        public async Task<IActionResult> DetailsSection(Guid documentId)
        {
            var model = await _documentService.GetDetailsSectionAsync(documentId);
            if (model == null)
                return Content("<div class='alert alert-warning'>Document not found.</div>", "text/html");

            return PartialView("_DocumentDetailsSectionPartial", model);
        }

        [HttpGet]
        [Authorize("Utilities.Documents.View")]
        public async Task<IActionResult> EmailDetails(Guid emailId)
        {
            var model = await _documentService.GetEmailDetailsAsync(emailId);
            if (model == null)
                return Content("<div class='modal-body'><div class='alert alert-warning'>Email not found.</div></div>", "text/html");

            return PartialView("_EmailDetailsModalPartial", model);
        }

        [HttpGet]
        [Authorize("Utilities.Documents.View")]
        public async Task<IActionResult> ViewAttachment(Guid attachmentId)
        {
            var result = await _documentService.GetAttachmentContentAsync(attachmentId);

            if (result.FileData != null && result.FileData.Length > 0)
            {
                return File(result.FileData, result.ContentType, result.FileName);
            }

            if (string.IsNullOrWhiteSpace(result.FilePath))
                return NotFound("Attachment file path is empty.");

            var filePath = result.FilePath.Trim();

            // اگر URL باشد
            if (Uri.TryCreate(filePath, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return Redirect(filePath);
            }
            return Redirect(result.FilePath);

            // اگر physical path باشد
            //if (Path.IsPathRooted(filePath))
            //{
            //    if (System.IO.File.Exists(filePath))
            //    {
            //        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //        return File(stream, result.ContentType, result.FileName);
            //    }
            //}
            //else
            //{
            // فرض: relative path زیر wwwroot

            var relativePath = filePath.TrimStart('~').TrimStart('/')
                                       .Replace("/", Path.DirectorySeparatorChar.ToString());

            var physicalPath = Path.Combine(_env.WebRootPath, relativePath);

            if (System.IO.File.Exists(physicalPath))
            {
                var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(stream, result.ContentType, result.FileName);
            }
            //}

            return NotFound($"Attachment file not found. Path: {filePath}");
        }

        private List<SelectListItem> GetPageSizeList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "10", Text = "10" },
                new SelectListItem { Value = "20", Text = "20" },
                new SelectListItem { Value = "50", Text = "50" },
                new SelectListItem { Value = "100", Text = "100" }
            };
        }
    }
}

