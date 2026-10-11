using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.RouteDTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Rate;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    // Repositories/RouteRepository.cs
    public class RouteRepository : IRouteRepository
    {
        private readonly ApplicationDbContext _context;

        public RouteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<RouteDto>> SearchAsync(RouteSearchDto searchDto)
        {
            var query = _context.Routes
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .Include(r => r.RouteMode)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                var term = searchDto.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.RouteName.ToLower().Contains(term) ||
                    r.Origin.LocationName.ToLower().Contains(term) ||
                    r.Destination.LocationName.ToLower().Contains(term));
            }

            if (searchDto.OriginId.HasValue)
                query = query.Where(r => r.OriginId == searchDto.OriginId.Value);

            if (searchDto.DestinationId.HasValue)
                query = query.Where(r => r.DestinationId == searchDto.DestinationId.Value);


            if (!string.IsNullOrEmpty(searchDto.OriginName))
            {
                query = query.Where(r => r.Origin.LocationName.Contains(searchDto.OriginName));
            }
            if (!string.IsNullOrEmpty(searchDto.DestinationName))
            {
                query = query.Where(r => r.Destination.LocationName.Contains(searchDto.DestinationName));
            }

            if (searchDto.RouteModeId.HasValue)
                query = query.Where(r => r.RouteModeId == searchDto.RouteModeId.Value);

            //if (searchDto.IsActive.HasValue)
            //    query = query.Where(r => r.IsActive == searchDto.IsActive.Value);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "routename" => searchDto.SortDescending
                    ? query.OrderByDescending(r => r.RouteName)
                    : query.OrderBy(r => r.RouteName),
                "origin" => searchDto.SortDescending
                    ? query.OrderByDescending(r => r.Origin.LocationName)
                    : query.OrderBy(r => r.Origin.LocationName),
                "destination" => searchDto.SortDescending
                    ? query.OrderByDescending(r => r.Destination.LocationName)
                    : query.OrderBy(r => r.Destination.LocationName),
                "mode" => searchDto.SortDescending
                    ? query.OrderByDescending(r => r.RouteMode.RouteModeName)
                    : query.OrderBy(r => r.RouteMode.RouteModeName),
                "distance" => searchDto.SortDescending
                    ? query.OrderByDescending(r => r.DistanceKms)
                    : query.OrderBy(r => r.DistanceKms),
                _ => query.OrderBy(r => r.RouteName)
            };

            // Apply pagination
            var items = await query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(r => new RouteDto
                {
                    RouteId = r.Id,
                    RouteName = r.RouteName,
                    OriginId = r.OriginId,
                    OriginName = r.Origin.LocationName,
                    DestinationId = r.DestinationId,
                    DestinationName = r.Destination.LocationName,
                    RouteModeId = r.RouteModeId,
                    RouteModeName = r.RouteMode.RouteModeName,
                    DistanceMiles = r.DistanceMiles,
                    DistanceKms = r.DistanceKms,
                    TransitTime = r.TransitTime,
                    //IsActive = r.IsActive
                })
                .ToListAsync();

            return new PagedResultDto<RouteDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<RouteDto> GetByIdAsync(Guid id)
        {
            return await _context.Routes
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .Include(r => r.RouteMode)
                .Where(r => r.Id == id)
                .Select(r => new RouteDto
                {
                    RouteId = r.Id,
                    RouteName = r.RouteName,
                    OriginId = r.OriginId,
                    OriginName = r.Origin.LocationName,
                    DestinationId = r.DestinationId,
                    DestinationName = r.Destination.LocationName,
                    RouteModeId = r.RouteModeId,
                    RouteModeName = r.RouteMode.RouteModeName,
                    DistanceMiles = r.DistanceMiles,
                    DistanceKms = r.DistanceKms,
                    TransitTime = r.TransitTime,
                    //IsActive = r.IsActive
                })
            .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateAsync(CreateRouteDto dto)
        {
            var entity = new Route
            {
                RouteName = dto.RouteName,
                OriginId = dto.OriginId,
                DestinationId = dto.DestinationId,
                RouteModeId = dto.RouteModeId,
                DistanceMiles = dto.DistanceMiles,
                DistanceKms = dto.DistanceKms,
                TransitTime = dto.TransitTime,
                //IsActive = dto.IsActive
            };

            _context.Routes.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task UpdateAsync(UpdateRouteDto dto)
        {
            var entity = await _context.Routes.FindAsync(dto.RouteId);
            if (entity == null) return;

            entity.RouteName = dto.RouteName;
            entity.OriginId = dto.OriginId;
            entity.DestinationId = dto.DestinationId;
            entity.RouteModeId = dto.RouteModeId;
            entity.DistanceMiles = dto.DistanceMiles;
            entity.DistanceKms = dto.DistanceKms;
            entity.TransitTime = dto.TransitTime;
            //entity.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Routes.FindAsync(id);
            if (entity == null) return;

            _context.Routes.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}