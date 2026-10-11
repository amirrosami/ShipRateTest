using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;

namespace ShipRate.Application.Interfaces.Documents
{
    public interface IDocumentService
    {
        Task<PagedResultDto<DocumentListItemDto>> SearchAsync(DocumentSearchDto input);
        Task<DocumentDetailsSectionDto?> GetDetailsSectionAsync(Guid documentId);
        Task<DocumentEmailDto?> GetEmailDetailsAsync(Guid emailId);

        Task<(byte[]? FileData, string? FilePath, string FileName, string ContentType)> GetAttachmentContentAsync(Guid attachmentId);

        Task<List<SelectListItem>> GetCompanySelectListAsync();
        Task<List<SelectListItem>> GetDocumentTypeSelectListAsync();
        Task<List<SelectListItem>> GetObjectTypeSelectListAsync();
        List<SelectListItem> GetIsActiveSelectList();
    }
}
