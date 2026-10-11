using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.RouteDTOs;

namespace ShipRate.Application.Interfaces
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _repo;

        public RouteService(IRouteRepository repo)
        {
            _repo = repo;
        }

        public Task<PagedResultDto<RouteDto>> SearchAsync(RouteSearchDto search)
            => _repo.SearchAsync(search);

        public Task<RouteDto> GetAsync(Guid id)
            => _repo.GetByIdAsync(id);

        public Task<Guid> CreateAsync(CreateRouteDto dto)
            => _repo.CreateAsync(dto);

        public Task UpdateAsync(UpdateRouteDto dto)
            => _repo.UpdateAsync(dto);

        public Task DeleteAsync(Guid id)
            => _repo.DeleteAsync(id);
    }
}
