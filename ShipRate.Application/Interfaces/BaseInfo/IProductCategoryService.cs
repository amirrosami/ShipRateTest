using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IProductCategoryService
    {
        Task<PagedResultDto<ProductCategoryDto>> GetPagedAsync(PagedRequestDto request);
        Task<ProductCategoryDto?> GetByIdAsync(Guid id);

        Task<bool> SoftDeleteAsync(Guid id);

        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(ProductCategoryCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(ProductCategoryUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);

    }
}
