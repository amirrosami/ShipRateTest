using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.RouteDTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IRouteRepository
    {
        Task<PagedResultDto<RouteDto>> SearchAsync(RouteSearchDto searchDto);
        Task<RouteDto> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateRouteDto dto);
        Task UpdateAsync(UpdateRouteDto dto);
        Task DeleteAsync(Guid id);
    }
}
