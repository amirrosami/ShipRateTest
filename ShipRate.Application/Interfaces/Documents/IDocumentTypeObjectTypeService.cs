using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;

namespace ShipRate.Application.Interfaces.Documents
{
    public interface IDocumentTypeObjectTypeService
    {
        // Master List (System Object Types)
        Task<PagedResultDto<SystemObjectTypeDto>> GetPagedSystemObjectsAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<SystemObjectTypeDto?> GetSystemObjectByIdAsync(int id);

        // Detail List (Document Type Object Types)
        Task<List<DocumentTypeObjectTypeDto>> GetByObjectTypeIdAsync(int objectTypeId);
        Task<DocumentTypeObjectTypeInputDto?> GetByIdAsync(int id);
        Task CreateAsync(DocumentTypeObjectTypeInputDto input);
        Task UpdateAsync(DocumentTypeObjectTypeInputDto input);
        Task DeleteAsync(int id);

        // Lookups for Forms
        Task<List<KeyValuePair<int, string>>> GetDocumentTypeLookupAsync(int objectTypeId);
    }
}
