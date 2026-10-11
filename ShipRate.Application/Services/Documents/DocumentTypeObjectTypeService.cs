using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;
using ShipRate.Application.Interfaces.Documents;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Documents
{
    public class DocumentTypeObjectTypeService : IDocumentTypeObjectTypeService
    {
        private readonly ApplicationDbContext _context;

        public DocumentTypeObjectTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<SystemObjectTypeDto>> GetPagedSystemObjectsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.SystemObjectTypes
                .Where(x => !x.IsDeleted && x.IsActive)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(x => x.SystemObjectTypeName.ToLower().Contains(searchTerm) ||
                                         x.SystemObjectTypeLocalName.ToLower().Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.SystemObjectTypeName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SystemObjectTypeDto
                {
                    Id = x.Id,
                    SystemObjectTypeName = x.SystemObjectTypeName,
                    SystemObjectTypeLocalName = x.SystemObjectTypeLocalName
                })
                .ToListAsync();

            return new PagedResultDto<SystemObjectTypeDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<SystemObjectTypeDto?> GetSystemObjectByIdAsync(int id)
        {
            var entity = await _context.SystemObjectTypes.FindAsync(id);
            if (entity == null) return null;

            return new SystemObjectTypeDto
            {
                Id = entity.Id,
                SystemObjectTypeName = entity.SystemObjectTypeName,
                SystemObjectTypeLocalName = entity.SystemObjectTypeLocalName
            };
        }

        public async Task<List<DocumentTypeObjectTypeDto>> GetByObjectTypeIdAsync(int objectTypeId)
        {
            return await _context.DocumentTypeObjectTypes
                .Include(d => d.DocumentType)
                .Include(d => d.ObjectType)
                .Where(x => x.ObjectTypeId == objectTypeId && !x.IsDeleted && x.IsActive)
                .AsNoTracking()
                .Select(x => new DocumentTypeObjectTypeDto
                {
                    Id = x.Id,
                    DocumentTypeId = x.DocumentTypeId ?? 0,
                    DocumentTypeName = x.DocumentType.DocumentTypeName,
                    DocumentTypeLocalName = x.DocumentType.DocumentTypeLocalName,
                    ObjectTypeId = x.ObjectTypeId ?? 0,
                    ObjectTypeName = x.ObjectType.SystemObjectTypeName,
                    SystemDefined = x.SystemDefined
                })
                .ToListAsync();
        }

        public async Task<DocumentTypeObjectTypeInputDto?> GetByIdAsync(int id)
        {
            var entity = await _context.DocumentTypeObjectTypes.FindAsync(id);
            if (entity == null) return null;

            return new DocumentTypeObjectTypeInputDto
            {
                Id = entity.Id,
                DocumentTypeId = entity.DocumentTypeId ?? 0,
                ObjectTypeId = entity.ObjectTypeId ?? 0,
                SystemDefined = entity.SystemDefined
            };
        }

        public async Task CreateAsync(DocumentTypeObjectTypeInputDto input)
        {
            var entity = new DocumentTypeObjectType
            {
                DocumentTypeId = input.DocumentTypeId,
                ObjectTypeId = input.ObjectTypeId,
                SystemDefined = input.SystemDefined,
                IsActive = true,
                IsDeleted = false
            };

            _context.DocumentTypeObjectTypes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentTypeObjectTypeInputDto input)
        {
            if (!input.Id.HasValue) throw new ArgumentException("Id is required for update.");

            var entity = await _context.DocumentTypeObjectTypes.FindAsync(input.Id.Value);
            if (entity == null) throw new KeyNotFoundException("Mapping not found.");
            if (entity.SystemDefined) throw new InvalidOperationException("System defined records cannot be edited.");

            entity.DocumentTypeId = input.DocumentTypeId;
            entity.ObjectTypeId = input.ObjectTypeId;

            _context.DocumentTypeObjectTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.DocumentTypeObjectTypes.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException("Mapping not found.");
            if (entity.SystemDefined) throw new InvalidOperationException("System defined records cannot be deleted.");

            // سافت دیلیت یا هارد دیلیت بسته به معماری پروژه
            entity.IsDeleted = true;
            _context.DocumentTypeObjectTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetDocumentTypeLookupAsync(int objectTypeId)
        {
            return await _context.DocumentTypes
                .AsNoTracking()
                .Where(d => !_context.DocumentTypeObjectTypes.Any(x =>
                    x.ObjectTypeId == objectTypeId &&
                    x.DocumentTypeId == d.Id &&
                    x.IsActive &&
                    !x.IsDeleted))
                .OrderBy(d => d.DocumentTypeName)
                .Select(d => new KeyValuePair<int, string>(d.Id, d.DocumentTypeName))
                .ToListAsync();
        }
    }
}
