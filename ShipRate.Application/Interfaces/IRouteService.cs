using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.RouteDTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IRouteService
    {
        Task<PagedResultDto<RouteDto>> SearchAsync(RouteSearchDto search);
        Task<RouteDto> GetAsync(Guid id);
        Task<Guid> CreateAsync(CreateRouteDto dto);
        Task UpdateAsync(UpdateRouteDto dto);
        Task DeleteAsync(Guid id);
    }
}
