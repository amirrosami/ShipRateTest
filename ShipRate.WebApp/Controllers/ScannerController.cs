using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class ScannerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFromScanner(IFormFile RemoteFile)
        {
            if (RemoteFile == null || RemoteFile.Length == 0)
                return BadRequest("No file received.");

            var scansFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "scans");

            if (!Directory.Exists(scansFolder))
                Directory.CreateDirectory(scansFolder);

            var extension = Path.GetExtension(RemoteFile.FileName);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg";

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(scansFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await RemoteFile.CopyToAsync(stream);

            return Json(new
            {
                success = true,
                fileName,
                url = Url.Content("~/scans/" + fileName)
            });
        }
    }
}

