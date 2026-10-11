using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Security;
using ShipRate.Application.Utilities;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DocumentManagerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPdfExtractionService _pdfService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAttachmentSecurityService _securityService;
        private readonly IImageUtility _imageUtility;
        public DocumentManagerController(ApplicationDbContext db, IPdfExtractionService pdfService, ICurrentUserService currentUserService, IAttachmentSecurityService securityService, IImageUtility imageUitility)
        {
            _db = db;
            _pdfService = pdfService;
            _currentUserService = currentUserService;
            _securityService = securityService;
            _imageUtility = imageUitility;
        }
        // GET: Main modal content
        [HttpGet]
        public async Task<IActionResult> Index(Guid objectId, int objectTypeId, bool showSubsets)
        {
            var userId = _currentUserService.UserId;
            var docsQuery = _db.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Attachments)
                .Include(d => d.Emails)
                .Include(x => x.CreateUser)
                .Include(x => x.EditUser)
                .Include(x => x.ObjectType)
                .AsNoTracking();







            //query = query.Where(x => (x.EntityTypeId == entityTypeId && x.EntityId == entityId)
            //    || (x.EntityTypeId == 5 && x.EntityId.HasValue && bls.Contains(x.EntityId.Value))
            //    || (x.EntityTypeId == 2 && x.EntityId.HasValue && bookings.Contains(x.EntityId.Value))
            //    || (x.EntityTypeId == 11 && x.EntityId.HasValue && invoices.Contains(x.EntityId.Value))
            //    || (x.EntityTypeId == 12 && x.EntityId.HasValue && charges.Contains(x.EntityId.Value))
            //    );







            if (objectTypeId == 300 && showSubsets)
            {

                var bls = _db.BLs
                    .Where(x => x.JobId == objectId)
                    .Select(x => x.Id).ToList();

                var bookings = _db.Bookings
                    .Where(x => x.JobId == objectId)
                    .Select(x => x.Id).ToList();

                var invoices = _db.Invoices
                    .Where(x => x.JobId == objectId)
                    .Select(x => x.Id).ToList();

                //var charges = _db.Charges
                //    .Where(x => x.JobId == objectId)
                //    .Select(x => x.Id);

                docsQuery = docsQuery.Where(d => (d.ObjectId == objectId && d.ObjectTypeId == objectTypeId)
                                || (d.ObjectTypeId == 5 && bls.Contains(d.ObjectId))
                                || (d.ObjectTypeId == 6 && bookings.Contains(d.ObjectId))
                                || (d.ObjectTypeId == 200 && invoices.Contains(d.ObjectId))
                );
            }
            else
            {
                docsQuery = docsQuery.Where(d => d.ObjectId == objectId && d.ObjectTypeId == objectTypeId && !d.IsDeleted);
            }

            if (User.IsInRole("Customer"))
            {
                docsQuery = docsQuery.Where(x => x.CreateUserId == userId);
            }
            var docs = await docsQuery.OrderByDescending(d => d.CreatedAt)
            .Select(d => new DocumentListItem
            {
                Id = d.Id,
                DocumentType = d.DocumentType != null ? d.DocumentType.DocumentTypeName : "",
                DocumentTitle = d.DocumentTitle,
                DocumentNo = d.DocumentNo,
                DocumentDate = d.DocumentDate,
                Description = d.DocumentDescription,
                CreateUserName = d.CreateUser.FullName,
                LastEditUserName = d.EditUser.FullName,
                CreateDateTime = d.CreatedAt,
                LastEditDateTime = d.UpdatedAt,
                AttachmentCount = d.Attachments.Count(a => !a.IsDeleted),
                EmailCount = d.Emails.Count(e => !e.IsDeleted),
                IsLast = d.IsLast,
                ObjectTypeName = d.ObjectType.SystemObjectTypeName

            })
            .ToListAsync();
            var vm = new DocumentManagerViewModel
            {
                ObjectId = objectId,
                ObjectTypeId = objectTypeId,
                Documents = docs,
                ShowSubset = showSubsets
            };
            return PartialView("_DocumentManager", vm);
        }

        // GET: Document form (create/edit)
        [HttpGet]
        public async Task<IActionResult> DocumentForm(Guid objectId, int objectTypeId, Guid? id)
        {
            var types = await _db.DocumentTypes
                .Select(t => new LookupItem { Id = t.Id, Title = t.DocumentTypeName })
                .ToListAsync();

            if (!types.Any())
            {
                types = await _db.DocumentTypes
                    .Select(t => new LookupItem { Id = t.Id, Title = t.DocumentTypeName })
                    .ToListAsync();
            }

            var vm = new DocumentFormViewModel
            {
                ObjectId = objectId,
                ObjectTypeId = objectTypeId,
                DocumentTypes = types
            };

            if (id.HasValue)
            {
                var doc = await _db.Documents.FindAsync(id.Value);
                if (doc != null)
                {
                    vm.Id = doc.Id;
                    vm.DocumentTypeId = doc.DocumentTypeId;
                    vm.DocumentTitle = doc.DocumentTitle;
                    vm.DocumentNo = doc.DocumentNo;
                    vm.DocumentDate = doc.DocumentDate;
                    vm.DocumentDescription = doc.DocumentDescription;
                    vm.IsLast = doc.IsLast;
                }
            }

            return PartialView("_DocumentForm", vm);
        }

        // POST: Save document
        [HttpPost]
        public async Task<IActionResult> SaveDocument(DocumentFormViewModel vm)
        {

            var id = Guid.Empty;
            if (vm.Id.HasValue && vm.Id != Guid.Empty)
            {
                var doc = await _db.Documents.FindAsync(vm.Id.Value);
                if (doc != null)
                {
                    id = vm.Id.Value;
                    doc.DocumentTypeId = vm.DocumentTypeId;
                    doc.DocumentTitle = vm.DocumentTitle;
                    doc.DocumentNo = vm.DocumentNo;
                    doc.DocumentDate = vm.DocumentDate;
                    doc.DocumentDescription = vm.DocumentDescription;
                    doc.IsLast = vm.IsLast;
                    doc.EditUserId = _currentUserService.UserId;
                    doc.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                var doc = new Document
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = 1,
                    ObjectId = vm.ObjectId,
                    ObjectTypeId = vm.ObjectTypeId,
                    DocumentTypeId = vm.DocumentTypeId,
                    DocumentTitle = vm.DocumentTitle,
                    DocumentNo = vm.DocumentNo,
                    DocumentDate = vm.DocumentDate,
                    DocumentDescription = vm.DocumentDescription,
                    //IsActive = true,
                    CreateUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.Now,
                    IsLast = vm.IsLast
                };
                _db.Documents.Add(doc);
                id = doc.Id;
            }
            await _db.SaveChangesAsync();
            if (vm.IsLast) await UpdateOtherIsLastsAsync(id, vm.ObjectId, vm.ObjectTypeId);
            return await Index(vm.ObjectId, vm.ObjectTypeId, false);
        }


        public async Task UpdateOtherIsLastsAsync(Guid id, Guid objectId, int objectTypeId)
        {
            var otherdocs = await _db.Documents.Where(x => x.ObjectId == objectId && x.ObjectTypeId == objectTypeId && x.Id != id).ToListAsync();
            if (otherdocs != null)
            {
                foreach (var item in otherdocs)
                {
                    item.IsLast = false;
                }
            }
            await _db.SaveChangesAsync();
        }
        // POST: Delete document
        [HttpPost]
        public async Task<IActionResult> DeleteDocument(Guid id, Guid objectId, int objectTypeId)
        {
            var doc = await _db.Documents.FindAsync(id);
            if (doc != null)
            {
                doc.IsDeleted = true;
                doc.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            return await Index(objectId, objectTypeId, false);
        }

        // GET: Attachments grid for a document
        [HttpGet]
        public async Task<IActionResult> Attachments(Guid documentId)
        {
            var items = await _db.DocumentAttachments
                .Where(a => a.DocumentId == documentId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AttachmentListItem
                {
                    Id = a.Id,
                    FileName = a.DocumentAttachmentFileName,
                    FileType = a.DocumentAttachmentFileType,
                    FileExtension = a.DocumentAttachmentFileExtension,
                    FileSize = a.DocumentAttachmentFileSize,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            ViewBag.DocumentId = documentId;
            return PartialView("_AttachmentsGrid", items);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAttachment(Guid documentId, IFormFile file, CancellationToken cancellationToken)
        {
            // 1. اعتبارسنجی امنیتی و اسکن بدافزار
            var securityResult = await _securityService.ValidateAndScanAsync(file, cancellationToken);

            // فرض: نتیجه شامل IsAllowed (bool) و Reason (string) و غیره است
            if (!securityResult.IsAllowed)
            {
                return BadRequest(securityResult.RejectionReason);
            }

            // 2. تعیین نوع پردازش
            var isImage = securityResult.Extension is ".jpg" or ".jpeg" or ".png" or ".webp";
            byte[] fileData;

            try
            {
                if (isImage)
                {
                    // استفاده از سرویس فشرده‌سازی برای تصاویر
                    fileData = await _imageUtility.CompressImageAsync(file, cancellationToken);
                }
                else
                {
                    // خواندن مستقیم فایل‌های غیر تصویری (PDF, Docx, etc)
                    await using var input = file.OpenReadStream();
                    using var ms = new MemoryStream();
                    await input.CopyToAsync(ms, cancellationToken);
                    fileData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                // لاگ خطا در صورت بروز مشکل در پردازش تصویر
                return BadRequest("خطا در پردازش فایل: " + ex.Message);
            }

            // 3. آماده‌سازی مسیر و ذخیره‌سازی روی دیسک
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
            Directory.CreateDirectory(uploadsDir);

            // اگر تصویر است، پسوند جدید webp خواهد بود
            var storedExtension = isImage ? ".webp" : securityResult.Extension;
            var uniqueName = $"{Guid.CreateVersion7()}{storedExtension}";
            var physicalPath = Path.Combine(uploadsDir, uniqueName);

            await System.IO.File.WriteAllBytesAsync(physicalPath, fileData, cancellationToken);

            // 4. ذخیره در دیتابیس
            var attachment = new DocumentAttachment
            {
                Id = Guid.CreateVersion7(),
                DocumentId = documentId,
                DocumentAttachmentFileName = securityResult.SafeOriginalFileName,
                DocumentAttachmentFileType = isImage ? "image/webp" : securityResult.ContentType,
                DocumentAttachmentFileExtension = storedExtension,
                DocumentAttachmentFileSize = fileData.Length / 1024m, // KB
                DocumentAttachmentFileData = fileData,
                DocumentAttachmentFilePath = $"/uploads/documents/{uniqueName}",
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };

            _db.DocumentAttachments.Add(attachment);
            await _db.SaveChangesAsync(cancellationToken);

            // 5. بازگشت به نمای پیوست‌ها
            return await Attachments(documentId);
        }
        // POST: Upload attachment
        //[HttpPost]
        //public async Task<IActionResult> UploadAttachment(Guid documentId, IFormFile file)
        //{
        //    if (file != null && file.Length > 0)
        //    {
        //        // Save to disk
        //        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(),
        //                                      "wwwroot", "uploads", "documents");
        //        Directory.CreateDirectory(uploadsDir);

        //        var uniqueName = $"{Guid.CreateVersion7()}{Path.GetExtension(file.FileName)}";
        //        var filePath = Path.Combine(uploadsDir, uniqueName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        //var extractedData = _pdfService.ExtractTextUsingPdfPig(filePath);



        //        var attachment = new DocumentAttachment
        //        {
        //            Id = Guid.CreateVersion7(),
        //            DocumentId = documentId,
        //            DocumentAttachmentFileName = file.FileName,
        //            DocumentAttachmentFileType = file.ContentType,
        //            DocumentAttachmentFileExtension = Path.GetExtension(file.FileName),
        //            DocumentAttachmentFileSize = file.Length / 1024m, // KB
        //            DocumentAttachmentFilePath = $"/uploads/documents/{uniqueName}",
        //            //IsActive = true,
        //            CreatedAt = DateTime.Now,
        //            CreateUserId = _currentUserService.UserId
        //        };

        //        _db.DocumentAttachments.Add(attachment);
        //        await _db.SaveChangesAsync();
        //    }

        //    return await Attachments(documentId);
        //}

        // POST: Delete attachment
        [HttpPost]
        public async Task<IActionResult> DeleteAttachment(Guid id, Guid documentId)
        {
            var att = await _db.DocumentAttachments.FindAsync(id);
            if (att != null)
            {
                att.IsDeleted = true;
                att.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            return await Attachments(documentId);
        }

        //// GET: Download attachment
        //[HttpGet]
        //public async Task<IActionResult> DownloadAttachment(Guid id)
        //{
        //    var att = await _db.DocumentAttachments.FindAsync(id);
        //    if (att == null) return NotFound();

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(),
        //                                "wwwroot", att.DocumentAttachmentFilePath.TrimStart('/'));
        //    if (!System.IO.File.Exists(filePath)) return NotFound();

        //    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        //    return File(bytes, att.DocumentAttachmentFileType, att.DocumentAttachmentFileName);
        //}



        //[HttpGet]
        //public async Task<IActionResult> DownloadAttachment(Guid id)
        //{
        //    var att = await _db.DocumentAttachments.FindAsync(id);
        //    if (att == null) return NotFound();

        //    byte[] fileData = null;
        //    string contentType = att.DocumentAttachmentFileType ?? "application/octet-stream";
        //    string fileName = att.DocumentAttachmentFileName;

        //    // 1. بررسی اولویت: اگر دیتا در دیتابیس موجود است
        //    if (att.DocumentAttachmentFileData != null && att.DocumentAttachmentFileData.Length > 0)
        //    {
        //        fileData = att.DocumentAttachmentFileData;
        //    }
        //    // 2. اگر دیتا در دیتابیس نبود، چک کردن فایل فیزیکی در مسیر سرور
        //    else if (!string.IsNullOrEmpty(att.DocumentAttachmentFilePath))
        //    {
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(),
        //                                    "wwwroot", att.DocumentAttachmentFilePath.TrimStart('/'));

        //        if (System.IO.File.Exists(filePath))
        //        {
        //            fileData = await System.IO.File.ReadAllBytesAsync(filePath);
        //        }
        //    }

        //    // اگر در هیچ‌کدام از حالت‌ها فایلی پیدا نشد
        //    if (fileData == null) return NotFound("File content not found.");

        //    // تنظیم هدر برای باز شدن در تب جدید (به جای دانلود اجباری)
        //    var contentDisposition = new System.Net.Mime.ContentDisposition
        //    {
        //        FileName = fileName,
        //        Inline = true // تلاش برای باز کردن در مرورگر
        //    };
        //    Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

        //    return File(fileData, contentType);
        //}

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(Guid id, CancellationToken cancellationToken)
        {
            // 1. فقط متادیتا خوانده می‌شود (فیلد سنگین DocumentAttachmentFileData واکشی نمی‌شود)
            var attMeta = await _db.DocumentAttachments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.DocumentAttachmentFileName,
                    x.DocumentAttachmentFileType,
                    x.DocumentAttachmentFilePath
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (attMeta == null)
                return NotFound();

            var contentType = attMeta.DocumentAttachmentFileType ?? "application/octet-stream";
            var fileName = attMeta.DocumentAttachmentFileName;

            // تنظیم هدر برای نمایش Inline (باز شدن در تب مرورگر)
            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = true
            };
            Response.Headers["Content-Disposition"] = contentDisposition.ToString();

            // 2. اولویت اول: بررسی فایل فیزیکی روی دیسک
            if (!string.IsNullOrEmpty(attMeta.DocumentAttachmentFilePath))
            {
                var physicalPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    attMeta.DocumentAttachmentFilePath.TrimStart('/', '\\')
                );

                if (System.IO.File.Exists(physicalPath))
                {
                    // ارسال مستقیم به صورت Stream و بدون بارگذاری کل بایت‌ها در رم
                    return PhysicalFile(physicalPath, contentType, enableRangeProcessing: true);
                }
            }

            // 3. اولویت دوم (Fallback): اگر روی دیسک نبود، فقط بایت‌های دیتابیس واکشی می‌شوند
            var fileData = await _db.DocumentAttachments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => x.DocumentAttachmentFileData)
                .FirstOrDefaultAsync(cancellationToken);

            if (fileData != null && fileData.Length > 0)
            {
                return File(fileData, contentType);
            }

            // اگر در هیچ‌کدام پیدا نشد
            return NotFound("فایل مورد نظر یافت نشد.");
        }









        // GET: Emails grid for a document
        [HttpGet]
        public async Task<IActionResult> Emails(Guid documentId)
        {
            var items = await _db.DocumentEmails
                .Where(e => e.DocumentId == documentId && !e.IsDeleted)
                .OrderByDescending(e => e.DocumentEmailSendDate)
                .Select(e => new EmailListItem
                {
                    Id = e.Id,
                    Subject = e.DocumentEmailSubject,
                    From = e.DocumentEmailFrom,
                    To = e.DocumentEmailTo,
                    SendDate = e.DocumentEmailSendDate,
                    Direction = e.EmailDirectionId == 1 ? "Inbound" : "Outbound"
                })
                .ToListAsync();

            ViewBag.DocumentId = documentId;
            return PartialView("_EmailsGrid", items);
        }

        // GET: Email form
        [HttpGet]
        public async Task<IActionResult> EmailForm(Guid documentId, Guid? id)
        {
            var vm = new EmailFormViewModel
            {
                DocumentId = documentId,
                EmailDirections = new List<LookupItem>
                {
                    new() { Id = 1, Title = "Inbound" },
                    new() { Id = 2, Title = "Outbound" }
                }
            };

            if (id.HasValue)
            {
                var email = await _db.DocumentEmails.FindAsync(id.Value);
                if (email != null)
                {
                    vm.Id = email.Id;
                    vm.Subject = email.DocumentEmailSubject;
                    vm.Body = email.DocumentEmailBody;
                    vm.To = email.DocumentEmailTo;
                    vm.CC = email.DocumentEmailCC;
                    vm.BCC = email.DocumentEmailBCC;
                    vm.EmailDirectionId = email.EmailDirectionId;
                }
            }

            return PartialView("_EmailForm", vm);
        }

        // POST: Save email
        [HttpPost]
        public async Task<IActionResult> SaveEmail(EmailFormViewModel vm)
        {
            if (vm.Id.HasValue && vm.Id != Guid.Empty)
            {
                var email = await _db.DocumentEmails.FindAsync(vm.Id.Value);
                if (email != null)
                {
                    email.DocumentEmailSubject = vm.Subject;
                    email.DocumentEmailBody = vm.Body;
                    email.DocumentEmailTo = vm.To;
                    email.DocumentEmailCC = vm.CC ?? "";
                    email.DocumentEmailBCC = vm.BCC ?? "";
                    email.EmailDirectionId = vm.EmailDirectionId;
                    email.UpdatedAt = DateTime.Now;
                }
            }
            else
            {
                var email = new DocumentEmail
                {
                    Id = Guid.CreateVersion7(),
                    DocumentId = vm.DocumentId,
                    DocumentEmailSubject = vm.Subject,
                    DocumentEmailBody = vm.Body,
                    DocumentEmailFromUserId = Guid.Empty, // TODO
                    DocumentEmailFrom = "", // TODO: current user email
                    DocumentEmailTo = vm.To,
                    DocumentEmailCC = vm.CC ?? "",
                    DocumentEmailBCC = vm.BCC ?? "",
                    DocumentEmailSendDate = DateTime.Now,
                    EmailDirectionId = vm.EmailDirectionId,
                    //IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                _db.DocumentEmails.Add(email);
            }

            await _db.SaveChangesAsync();
            return await Emails(vm.DocumentId);
        }

        // POST: Delete email
        [HttpPost]
        public async Task<IActionResult> DeleteEmail(Guid id, Guid documentId)
        {
            var email = await _db.DocumentEmails.FindAsync(id);
            if (email != null)
            {
                email.IsDeleted = true;
                email.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
            return await Emails(documentId);
        }
    }
}