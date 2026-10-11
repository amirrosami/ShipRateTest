using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Documents;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Documents;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Documents
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DocumentTypeService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResultDto<DocumentTypeListDto>> GetPagedListAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.DocumentTypes
                .Include(d => d.DocumentTypeCategory)
                .Include(d => d.CreateUser)
                .Include(d => d.UpdateUser)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(d => d.DocumentTypeName.ToLower().Contains(searchTerm) ||
                                         d.DocumentTypeLocalName.ToLower().Contains(searchTerm) ||
                                         d.DocumentTypeCategory.DocumentTypeCategoryName.ToLower().Contains(searchTerm));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.DocumentTypeName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DocumentTypeListDto
                {
                    Id = d.Id,
                    DocumentTypeName = d.DocumentTypeName,
                    CategoryName = d.DocumentTypeCategory.DocumentTypeCategoryName,
                    SystemDefined = d.SystemDefined,
                    HasExpiryDate = d.HasExpiryDate,
                    AlarmDaysBeforeExpire = d.AlarmDaysBeforeExpire,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    CreateUserName = d.CreateUser == null ? "System" : d.CreateUser.UserName,
                    UpdateUserName = d.UpdateUser == null ? "" : d.UpdateUser.UserName,
                    DocumentTypeLocalName = d.DocumentTypeLocalName
                })
                .ToListAsync();

            return new PagedResultDto<DocumentTypeListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<DocumentTypeInputDto?> GetByIdAsync(int id)
        {
            var docType = await _context.DocumentTypes.FindAsync(id);
            if (docType == null) return null;

            return new DocumentTypeInputDto
            {
                Id = docType.Id,
                DocumentTypeCategoryId = docType.DocumentTypeCategoryId,
                DocumentTypeName = docType.DocumentTypeName,
                DocumentTypeLocalName = docType.DocumentTypeLocalName,
                SystemDefined = docType.SystemDefined,
                HasExpiryDate = docType.HasExpiryDate,
                AlarmDaysBeforeExpire = docType.AlarmDaysBeforeExpire
            };
        }

        public async Task CreateAsync(DocumentTypeInputDto input)
        {
            var entity = new DocumentType
            {
                DocumentTypeCategoryId = input.DocumentTypeCategoryId,
                DocumentTypeName = input.DocumentTypeName,
                SystemDefined = input.SystemDefined,
                HasExpiryDate = input.HasExpiryDate,
                AlarmDaysBeforeExpire = input.AlarmDaysBeforeExpire,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,
                DocumentTypeLocalName = input.DocumentTypeLocalName,
                IsDeleted = false,
                IsActive = true
            };

            _context.DocumentTypes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentTypeInputDto input)
        {
            if (!input.Id.HasValue) throw new ArgumentException("Id is required for update.");

            var entity = await _context.DocumentTypes.FindAsync(input.Id.Value);
            if (entity == null) throw new KeyNotFoundException("Document type not found.");

            if (entity.SystemDefined) throw new InvalidOperationException("System defined document types cannot be edited.");
            entity.DocumentTypeCategoryId = input.DocumentTypeCategoryId;
            entity.DocumentTypeName = input.DocumentTypeName;
            entity.DocumentTypeLocalName = input.DocumentTypeLocalName;
            entity.HasExpiryDate = input.HasExpiryDate;
            entity.AlarmDaysBeforeExpire = input.AlarmDaysBeforeExpire;

            entity.UpdatedAt = DateTime.Now;
            entity.UpdateUserId = _currentUserService.UserId;

            _context.DocumentTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.DocumentTypes.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException("Document type not found.");
            if (entity.SystemDefined) throw new InvalidOperationException("System defined document types cannot be deleted.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = _currentUserService.UserId;

            _context.DocumentTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetCategoryLookupAsync()
        {
            return await _context.DocumentTypeCategories
                .AsNoTracking()
                .Select(c => new KeyValuePair<int, string>(c.Id, c.DocumentTypeCategoryName))
                .ToListAsync();
        }
    }
}
