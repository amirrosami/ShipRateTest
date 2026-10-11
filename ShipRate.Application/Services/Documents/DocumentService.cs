using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;
using ShipRate.Application.Interfaces.Documents;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Documents
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _db;

        public DocumentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultDto<DocumentListItemDto>> SearchAsync(DocumentSearchDto input)
        {
            var query = _db.Documents
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (input.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == input.CompanyId.Value);

            if (input.DocumentTypeId.HasValue)
                query = query.Where(x => x.DocumentTypeId == input.DocumentTypeId.Value);

            if (input.ObjectTypeId.HasValue)
                query = query.Where(x => x.ObjectTypeId == input.ObjectTypeId.Value);

            if (!string.IsNullOrWhiteSpace(input.DocumentTitle))
            {
                var title = input.DocumentTitle.Trim();
                query = query.Where(x => x.DocumentTitle.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(input.DocumentNo))
            {
                var docNo = input.DocumentNo.Trim();
                query = query.Where(x => x.DocumentNo.Contains(docNo));
            }

            if (input.DocumentDateFrom.HasValue)
                query = query.Where(x => x.DocumentDate >= input.DocumentDateFrom.Value);

            if (input.DocumentDateTo.HasValue)
            {
                var toDate = input.DocumentDateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.DocumentDate < toDate);
            }

            if (input.ExpiryDateFrom.HasValue)
                query = query.Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value >= input.ExpiryDateFrom.Value);

            if (input.ExpiryDateTo.HasValue)
            {
                var toDate = input.ExpiryDateTo.Value.Date.AddDays(1);
                query = query.Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value < toDate);
            }

            //if (input.IsActive.HasValue)
            //query = query.Where(x => x.IsActive == input.IsActive.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.DocumentDate)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new DocumentListItemDto
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Person.PersonName,
                    DocumentTypeId = x.DocumentTypeId,
                    DocumentTypeName = x.DocumentType.DocumentTypeName,
                    ObjectTypeId = x.ObjectTypeId,
                    ObjectTypeName = x.ObjectType.SystemObjectTypeName,
                    DocumentTitle = x.DocumentTitle,
                    DocumentNo = x.DocumentNo,
                    DocumentDate = x.DocumentDate,
                    DocumentDescription = x.DocumentDescription,
                    ObjectId = x.ObjectId,
                    ObjectDisplayText = x.ObjectId.ToString(),
                    ExpiryDate = x.ExpiryDate,
                    IsLast = x.IsLast

                    //IsActive = x.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<DocumentListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = input.PageNumber,
                PageSize = input.PageSize
            };
        }

        public async Task<DocumentDetailsSectionDto?> GetDetailsSectionAsync(Guid documentId)
        {
            var doc = await _db.Documents
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .AsNoTracking()
                .Where(x => x.Id == documentId && !x.IsDeleted)
                .Select(x => new DocumentDetailsSectionDto
                {
                    DocumentId = x.Id,
                    DocumentTitle = x.DocumentTitle,
                    DocumentNo = x.DocumentNo,
                    DocumentDate = x.DocumentDate,
                    DocumentDescription = x.DocumentDescription,
                    CompanyName = x.Company.Person.PersonName,
                    DocumentTypeName = x.DocumentType.DocumentTypeName,
                    ObjectTypeName = x.ObjectType.SystemObjectTypeName,
                    ObjectDisplayText = x.ObjectId.ToString(),
                    ExpiryDate = x.ExpiryDate,
                    IsLast = x.IsLast
                    //IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();

            if (doc == null)
                return null;

            doc.Attachments = await _db.Set<DocumentAttachment>()
                .AsNoTracking()
                .Where(x => x.DocumentId == documentId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new DocumentAttachmentDto
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    DocumentAttachmentFileName = x.DocumentAttachmentFileName,
                    DocumentAttachmentFileType = x.DocumentAttachmentFileType,
                    DocumentAttachmentFileExtension = x.DocumentAttachmentFileExtension,
                    DocumentAttachmentFileSize = x.DocumentAttachmentFileSize,
                    DocumentAttachmentFilePath = x.DocumentAttachmentFilePath,
                    CreatedAt = x.CreatedAt,
                    //IsActive = x.IsActive
                })
                .ToListAsync();

            doc.Emails = await _db.Set<DocumentEmail>()
                .AsNoTracking()
                .Where(x => x.DocumentId == documentId && !x.IsDeleted)
                .OrderByDescending(x => x.DocumentEmailSendDate)
                .Select(x => new DocumentEmailDto
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    DocumentEmailSubject = x.DocumentEmailSubject,
                    DocumentEmailBody = x.DocumentEmailBody,
                    DocumentEmailFromUserId = x.DocumentEmailFromUserId,
                    DocumentEmailFrom = x.DocumentEmailFrom,
                    DocumentEmailTo = x.DocumentEmailTo,
                    DocumentEmailCC = x.DocumentEmailCC,
                    DocumentEmailBCC = x.DocumentEmailBCC,
                    DocumentEmailSendDate = x.DocumentEmailSendDate,
                    EmailDirectionId = x.EmailDirectionId,
                    EmailDirectionName = x.EmailDirection.EmailDirectionName,
                    //IsActive = x.IsActive
                })
                .ToListAsync();

            return doc;
        }

        public async Task<DocumentEmailDto?> GetEmailDetailsAsync(Guid emailId)
        {
            return await _db.Set<DocumentEmail>()
                .AsNoTracking()
                .Where(x => x.Id == emailId && !x.IsDeleted)
                .Select(x => new DocumentEmailDto
                {
                    Id = x.Id,
                    DocumentId = x.DocumentId,
                    DocumentEmailSubject = x.DocumentEmailSubject,
                    DocumentEmailBody = x.DocumentEmailBody,
                    DocumentEmailFromUserId = x.DocumentEmailFromUserId,
                    DocumentEmailFrom = x.DocumentEmailFrom,
                    DocumentEmailTo = x.DocumentEmailTo,
                    DocumentEmailCC = x.DocumentEmailCC,
                    DocumentEmailBCC = x.DocumentEmailBCC,
                    DocumentEmailSendDate = x.DocumentEmailSendDate,
                    EmailDirectionId = x.EmailDirectionId,
                    EmailDirectionName = x.EmailDirection.EmailDirectionName,
                    //IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(byte[]? FileData, string? FilePath, string FileName, string ContentType)> GetAttachmentContentAsync(Guid attachmentId)
        {
            var item = await _db.Set<DocumentAttachment>()
                .AsNoTracking()
                .Where(x => x.Id == attachmentId && !x.IsDeleted)
                .Select(x => new
                {
                    x.DocumentAttachmentFileData,
                    x.DocumentAttachmentFilePath,
                    x.DocumentAttachmentFileName,
                    x.DocumentAttachmentFileType,
                    x.DocumentAttachmentFileExtension
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return (null, null, string.Empty, "application/octet-stream");

            var fileName = item.DocumentAttachmentFileName;
            if (!string.IsNullOrWhiteSpace(item.DocumentAttachmentFileExtension) &&
                !fileName.EndsWith(item.DocumentAttachmentFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                fileName += item.DocumentAttachmentFileExtension.StartsWith(".")
                    ? item.DocumentAttachmentFileExtension
                    : "." + item.DocumentAttachmentFileExtension;
            }

            var contentType = string.IsNullOrWhiteSpace(item.DocumentAttachmentFileType)
                ? "application/octet-stream"
                : item.DocumentAttachmentFileType;

            return (item.DocumentAttachmentFileData, item.DocumentAttachmentFilePath, fileName, contentType);
        }

        public async Task<List<SelectListItem>> GetCompanySelectListAsync()
        {
            return await _db.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDocumentTypeSelectListAsync()
        {
            return await _db.Set<DocumentType>()
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.DocumentTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DocumentTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetObjectTypeSelectListAsync()
        {
            return await _db.Set<SystemObjectType>()
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.SystemObjectTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.SystemObjectTypeName
                })
                .ToListAsync();
        }

        public List<SelectListItem> GetIsActiveSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All" },
                new SelectListItem { Value = "true", Text = "Active" },
                new SelectListItem { Value = "false", Text = "Inactive" }
            };
        }
    }
}
