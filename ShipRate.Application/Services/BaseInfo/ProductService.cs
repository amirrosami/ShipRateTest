using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class ProductService
    {
        private readonly IProductRepository _repository;
        private readonly IRepository<ProductCategory> _categoryrepository;
        public readonly ICurrentUserService _currentUserService;


        public ProductService(IProductRepository repository, IRepository<ProductCategory> categoryrepository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _categoryrepository = categoryrepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResultDto<ProductDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.ProductCode.ToLower().Contains(term) ||
                    x.ProductName.ToLower().Contains(term));
            }

            // Filter by IsActive
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<ProductDto>(items, totalCount, request.PageNumber, request.PageSize);
        }





        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithCategoryAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(ProductDto dto)
        {

            if (await _repository.AnyAsync(x => x.ProductCode == dto.ProductCode))
                return (false, $"Product Code '{dto.ProductCode}' already exists.");


            var entity = new Product
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                ProductCategoryId = dto.ProductCategoryId,
                ProductMinStock = dto.ProductMinStock,
                IsActive = true,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }





        public async Task<(bool Success, string? Error)> UpdateAsync(ProductUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Product not found.");


            entity.ProductCode = dto.ProductCode;
            entity.ProductName = dto.ProductName;
            entity.ProductCategoryId = dto.ProductCategoryId;
            entity.ProductMinStock = dto.ProductMinStock;

            entity.IsActive = dto.IsActive;
            entity.EditUserId = _currentUserService.UserId;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);
        }





        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.EditUserId = _currentUserService.UserId;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static ProductDto MapToDto(Product entity)
        {
            return new ProductDto
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                ProductMinStock = entity.ProductMinStock,
                ProductCategoryName = entity.ProductCategory.ProductCategoryName,
                ProductName = entity.ProductName,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                ProductCategoryId = entity.ProductCategoryId
            };
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Product not found.");

            entity.IsDeleted = true;
            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Product not found.");

            entity.IsActive = !entity.IsActive;
            entity.EditUserId = _currentUserService.UserId;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<IEnumerable<ProductCategory>> GetCategoriesAsync()
        {
            return await _categoryrepository.GetAllAsync();
        }
    }
}