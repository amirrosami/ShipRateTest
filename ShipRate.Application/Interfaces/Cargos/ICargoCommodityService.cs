using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Cargo;

namespace ShipRate.Application.Interfaces.Cargos
{
    public interface ICargoCommodityService
    {
        Task<PagedResultDto<CargoCommodityListDto>> GetPagedListAsync(CargoCommoditySearchDto searchDto);
        Task<CargoCommodityDto> GetByIdAsync(Guid id);
        Task<CargoCommodityDto> CreateAsync(CargoCommodityDto dto, Guid userId);
        Task UpdateAsync(CargoCommodityDto dto, Guid userId);
        Task DeleteAsync(Guid id);
    }
}
