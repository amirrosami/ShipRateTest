using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;

namespace ShipRate.Application.Interfaces.Documents
{
    public interface IDocumentTypeService
    {
        Task<PagedResultDto<DocumentTypeListDto>> GetPagedListAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<DocumentTypeInputDto?> GetByIdAsync(int id);
        Task CreateAsync(DocumentTypeInputDto input);
        Task UpdateAsync(DocumentTypeInputDto input);
        Task DeleteAsync(int id);
        Task<List<KeyValuePair<int, string>>> GetCategoryLookupAsync();
    }
}
