using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IPackageTypeService
    {
        Task<PagedResultDto<PackageTypeDto>> GetPagedAsync(PagedRequestDto request);
        Task<PagedResultDto<PackageTypeDto>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir);
        Task<PackageTypeDto?> GetByIdAsync(int id);

        Task<bool> SoftDeleteAsync(int id);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);

        Task<(bool Success, string? Error)> DeleteAsync(int id);
        Task<(bool Success, string? Error)> CreateAsync(PackageTypeCreateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(PackageTypeCreateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(int id);

        Task<IEnumerable<NLookupDto>> SearchPackageTypesAsync(string term, int take);

        //Task<PackageTypeDto> CreateAsync(PackageTypeCreateDto dto);
        //Task<PackageTypeDto?> UpdateAsync(PackageTypeUpdateDto dto);
        //Task<bool> ToggleActiveAsync(int id);



    }
}
