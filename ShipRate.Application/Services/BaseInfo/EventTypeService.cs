using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.BaseInfo
{
    public class EventTypeService : IEventTypeService
    {
        private readonly ApplicationDbContext _context;

        public EventTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventTypeCategoryDto>> GetCategoriesAsync()
        {
            return await _context.EventTypeCategories
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.EventTypeCategoryName)
                .Select(x => new EventTypeCategoryDto
                {
                    Id = x.Id,
                    EventTypeCategoryName = x.EventTypeCategoryName,
                    EventTypeCategoryLocalName = x.EventTypeCategoryLocalName
                })
                .ToListAsync();
        }

        public async Task<PagedResultDto<EventTypeListItemDto>> GetPagedAsync(EventTypeSearchDto input)
        {
            var query = _context.EventTypes
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => !x.IsDeleted);

            if (input.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == input.CategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                var term = input.SearchTerm.Trim();

                query = query.Where(x =>
                    x.EventTypeCode.Contains(term) ||
                    x.EventTypeName.Contains(term) ||
                    x.EventTypeRemarks.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.EventTypeName)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new EventTypeListItemDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.EventTypeCategoryName : string.Empty,
                    EventTypeCode = x.EventTypeCode,
                    EventTypeName = x.EventTypeName,
                    EventTypeRemarks = x.EventTypeRemarks,
                    SystemDefined = x.SystemDefined,
                    IsSystemEvent = x.IsSystemEvent,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<EventTypeListItemDto>(items, totalCount, input.PageNumber, input.PageSize)
            {
                SearchTerm = input.SearchTerm
            };
        }

        public async Task<CreateEventTypeDto> GetCreateDtoAsync(int? categoryId)
        {
            return await System.Threading.Tasks.Task.FromResult(new CreateEventTypeDto
            {
                CategoryId = categoryId,
                IsActive = true
            });
        }

        public async Task<UpdateEventTypeDto?> GetUpdateDtoAsync(int id)
        {
            return await _context.EventTypes
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new UpdateEventTypeDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    EventTypeCode = x.EventTypeCode,
                    EventTypeName = x.EventTypeName,
                    EventTypeRemarks = x.EventTypeRemarks,
                    SystemDefined = x.SystemDefined,
                    IsSystemEvent = x.IsSystemEvent,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<EventTypeFormViewModel> GetCreateFormAsync(int? categoryId)
        {
            var dto = await GetCreateDtoAsync(categoryId);
            var categories = await GetCategorySelectListAsync(dto.CategoryId);

            return new EventTypeFormViewModel
            {
                CreateDto = dto,
                Categories = categories
            };
        }

        public async Task<EventTypeFormViewModel?> GetEditFormAsync(int id)
        {
            var dto = await GetUpdateDtoAsync(id);
            if (dto == null)
                return null;

            var categories = await GetCategorySelectListAsync(dto.CategoryId);

            return new EventTypeFormViewModel
            {
                UpdateDto = dto,
                Categories = categories
            };
        }

        public async Task<(bool Success, string Message)> CreateAsync(CreateEventTypeDto dto)
        {
            var categoryExists = await _context.EventTypeCategories
                .AnyAsync(x => x.Id == dto.CategoryId && !x.IsDeleted && x.IsActive);

            if (!categoryExists)
                return (false, "Selected category is invalid.");

            var duplicateCode = await _context.EventTypes
                .AnyAsync(x => !x.IsDeleted && x.EventTypeCode == dto.EventTypeCode);

            if (duplicateCode)
                return (false, "Event type code already exists.");

            var entity = new EventType
            {
                CategoryId = dto.CategoryId,
                EventTypeCode = dto.EventTypeCode.Trim(),
                EventTypeName = dto.EventTypeName.Trim(),
                EventTypeRemarks = dto.EventTypeRemarks.Trim(),
                SystemDefined = false,
                IsSystemEvent = false,
                IsActive = dto.IsActive,
                IsDeleted = false
            };

            _context.EventTypes.Add(entity);
            await _context.SaveChangesAsync();

            return (true, "Event type created successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(UpdateEventTypeDto dto)
        {
            var entity = await _context.EventTypes
                .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted);

            if (entity == null)
                return (false, "Event type not found.");

            var categoryExists = await _context.EventTypeCategories
                .AnyAsync(x => x.Id == dto.CategoryId && !x.IsDeleted && x.IsActive);

            if (!categoryExists)
                return (false, "Selected category is invalid.");

            var duplicateCode = await _context.EventTypes
                .AnyAsync(x => x.Id != dto.Id && !x.IsDeleted && x.EventTypeCode == dto.EventTypeCode);

            if (duplicateCode)
                return (false, "Event type code already exists.");

            entity.CategoryId = dto.CategoryId;
            entity.EventTypeCode = dto.EventTypeCode.Trim();
            entity.EventTypeName = dto.EventTypeName.Trim();
            entity.EventTypeRemarks = dto.EventTypeRemarks.Trim();
            //entity.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();

            return (true, "Event type updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            var entity = await _context.EventTypes
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return (false, "Event type not found.");

            entity.IsDeleted = true;
            await _context.SaveChangesAsync();

            return (true, "Event type deleted successfully.");
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync(int? selectedId = null)
        {
            return await _context.EventTypeCategories
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.EventTypeCategoryName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.EventTypeCategoryName,
                    Selected = selectedId.HasValue && selectedId.Value == x.Id
                })
                .ToListAsync();
        }
    }
}