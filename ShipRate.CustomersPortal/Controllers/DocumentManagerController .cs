using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    public class DocumentManagerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IPdfExtractionService _pdfService;
        private readonly ICurrentUserService _currentUserService;
        public DocumentManagerController(ApplicationDbContext db, IPdfExtractionService pdfService, ICurrentUserService currentUserService)
        {
            _db = db;
            _pdfService = pdfService;
            _currentUserService = currentUserService;
        }
        // GET: Main modal content
        [HttpGet]
        public async Task<IActionResult> Index(Guid objectId, int objectTypeId)
        {
            var userId = _currentUserService.UserId;
            var docsQuery = _db.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.Attachments)
                .Include(d => d.Emails)
                .Include(x => x.CreateUser)
                .Include(x => x.EditUser)
                .AsNoTracking()
                .Where(d => d.ObjectId == objectId && d.ObjectTypeId == objectTypeId && !d.IsDeleted);

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
                EmailCount = d.Emails.Count(e => !e.IsDeleted)
            })
            .ToListAsync();
            var vm = new DocumentManagerViewModel
            {
                ObjectId = objectId,
                ObjectTypeId = objectTypeId,
                Documents = docs
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
                }
            }

            return PartialView("_DocumentForm", vm);
        }

        // POST: Save document
        [HttpPost]
        public async Task<IActionResult> SaveDocument(DocumentFormViewModel vm)
        {
            if (vm.Id.HasValue && vm.Id != Guid.Empty)
            {
                var doc = await _db.Documents.FindAsync(vm.Id.Value);
                if (doc != null)
                {
                    doc.DocumentTypeId = vm.DocumentTypeId;
                    doc.DocumentTitle = vm.DocumentTitle;
                    doc.DocumentNo = vm.DocumentNo;
                    doc.DocumentDate = vm.DocumentDate;
                    doc.DocumentDescription = vm.DocumentDescription;
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
                };
                _db.Documents.Add(doc);
            }

            await _db.SaveChangesAsync();
            return await Index(vm.ObjectId, vm.ObjectTypeId);
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
            return await Index(objectId, objectTypeId);
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

        // POST: Upload attachment
        [HttpPost]
        public async Task<IActionResult> UploadAttachment(Guid documentId, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Save to disk
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(),
                                              "wwwroot", "uploads", "documents");
                Directory.CreateDirectory(uploadsDir);

                var uniqueName = $"{Guid.CreateVersion7()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsDir, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //var extractedData = _pdfService.ExtractTextUsingPdfPig(filePath);



                var attachment = new DocumentAttachment
                {
                    Id = Guid.CreateVersion7(),
                    DocumentId = documentId,
                    DocumentAttachmentFileName = file.FileName,
                    DocumentAttachmentFileType = file.ContentType,
                    DocumentAttachmentFileExtension = Path.GetExtension(file.FileName),
                    DocumentAttachmentFileSize = file.Length / 1024m, // KB
                    DocumentAttachmentFilePath = $"/uploads/documents/{uniqueName}",
                    //IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };

                _db.DocumentAttachments.Add(attachment);
                await _db.SaveChangesAsync();
            }

            return await Attachments(documentId);
        }

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

        // GET: Download attachment
        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(Guid id)
        {
            var att = await _db.DocumentAttachments.FindAsync(id);
            if (att == null) return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                                        "wwwroot", att.DocumentAttachmentFilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, att.DocumentAttachmentFileType, att.DocumentAttachmentFileName);
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