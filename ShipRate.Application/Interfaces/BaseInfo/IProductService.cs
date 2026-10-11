using ShipRate.Application.DTOs;
using ShipRate.Domain.Entities.InventoryOperation;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> GetPagedAsync(PagedRequestDto request);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(ProductCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(ProductUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);
        Task<IEnumerable<ProductCategory>> GetCategoriesAsync();
    }
}
