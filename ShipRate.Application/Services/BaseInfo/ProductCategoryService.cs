using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IRepository<ProductCategory> _repository;
        private readonly ICurrentUserService _cuurentUserService;


        public ProductCategoryService(IRepository<ProductCategory> repository, ICurrentUserService cuurentUserService)
        {
            _repository = repository;
            _cuurentUserService = cuurentUserService;
        }

        public async Task<PagedResultDto<ProductCategoryDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.ProductCategoryName.ToLower().Contains(term));
            }

            // Filter by IsActive
            //if (request.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == request.IsActive.Value);
            //}

            // Sorting
            //query = request.SortColumn?.ToLower() switch
            //{
            //    "ProductCategoryname" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.ProductCategoryName)
            //        : query.OrderBy(x => x.ProductCategoryName),
            //};

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<ProductCategoryDto>(items, totalCount, request.PageNumber, request.PageSize);
        }





        public async Task<ProductCategoryDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(ProductCategoryCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.ProductCategoryName == dto.ProductCategoryName))
                return (false, $"Category '{dto.ProductCategoryName}' already exists.");


            var entity = new ProductCategory
            {
                ProductCategoryName = dto.ProductCategoryName.Trim(),
                //IsActive = dto.IsActive,
                IsDeleted = false,
                CreateUserId = _cuurentUserService.UserId
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }




        public async Task<(bool Success, string? Error)> UpdateAsync(ProductCategoryUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Product Category not found.");



            entity.ProductCategoryName = dto.ProductCategoryName.Trim();
            //entity.IsActive = dto.IsActive;
            entity.EditUserId = _cuurentUserService.UserId;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            return (true, null);
        }





        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.EditUserId = _cuurentUserService.UserId;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }


        private static ProductCategoryDto MapToDto(ProductCategory entity)
        {
            return new ProductCategoryDto
            {
                Id = entity.Id,
                ProductCategoryName = entity.ProductCategoryName,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted
            };
        }


        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Product Category not found.");

            entity.IsDeleted = true;
            entity.EditUserId = _cuurentUserService.UserId;

            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Product Category not found.");

            entity.IsActive = !entity.IsActive;
            entity.EditUserId = _cuurentUserService.UserId;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}
