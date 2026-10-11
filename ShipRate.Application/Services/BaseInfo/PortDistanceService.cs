using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Interfaces;
using static ShipRate.Application.DTOs.PortDistanceDto;

namespace ShipRate.Application.Services.BaseInfo
{
    public class PortDistanceService : IPortDistanceService
    {
        private readonly IPortDistanceRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public PortDistanceService(IPortDistanceRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResultDto<PortDistanceDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.FromPort.LocationName.ToLower().Contains(term) ||
                    x.FromPort.LocationLocalName.ToLower().Contains(term) ||
                    x.FromPort.LocationCode.ToLower().Contains(term) ||
                    x.ToPort.LocationName.ToLower().Contains(term) ||
                    x.ToPort.LocationLocalName.ToLower().Contains(term) ||
                    x.ToPort.LocationCode.ToLower().Contains(term)
                    );
            }

            // Filter by IsActive
            //if (request.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == request.IsActive.Value);
            //}

            // Sorting
            //query = request.SortColumn?.ToLower() switch
            //{
            //    "packagetypecode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.PackageTypeCode)
            //        : query.OrderBy(x => x.PackageTypeCode),
            //    "packagetypename" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.PackageTypeName)
            //        : query.OrderBy(x => x.PackageTypeName),
            //    "numericcode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.NumericCode)
            //        : query.OrderBy(x => x.NumericCode),
            //    "enscode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.ENSCode)
            //        : query.OrderBy(x => x.ENSCode),
            //    "isactive" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.IsActive)
            //        : query.OrderBy(x => x.IsActive),
            //    _ => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.Sequence)
            //        : query.OrderBy(x => x.Sequence)
            //};

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<PortDistanceDto>(items, totalCount, request.PageNumber, request.PageSize);
        }




        public async Task<PortDistanceDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithRelationsAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(PortDistanceCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.FromPortId == dto.ToPortId))
                return (false, $"Locations Distance  already exists.");


            var entity = new PortDistance
            {
                FromPortId = dto.FromPortId,
                ToPortId = dto.ToPortId,
                AvgTransitTimeDays = dto.AvgDays,
                AvgTransitTimeHours = dto.AvgHours,
                DistanceKM = dto.DistanceKM,
                DistanceNM = dto.DistanceNM,
                Source = dto.Source,
                TransportTypeId = dto.TransPortTypeId,
                //IsActive = true,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }





        public async Task<(bool Success, string? Error)> UpdateAsync(PortDistanceUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Distance not found.");

            entity.FromPortId = dto.FromPortId;
            entity.ToPortId = dto.ToPortId;
            entity.AvgTransitTimeDays = dto.AvgDays;
            entity.AvgTransitTimeHours = dto.AvgHours;
            entity.DistanceKM = dto.DistanceKM;
            entity.DistanceNM = dto.DistanceNM;
            entity.Source = dto.Source;
            entity.TransportTypeId = dto.TransPortTypeId;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService?.UserId;
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
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService?.UserId;



            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static PortDistanceDto MapToDto(PortDistance entity)
        {
            return new PortDistanceDto
            {
                Id = entity.Id,
                FromPortId = entity.FromPortId,
                ToPortId = entity.ToPortId,
                AvgDays = entity.AvgTransitTimeDays.Value,
                AvgHours = entity.AvgTransitTimeHours.Value,
                DistanceKM = entity.DistanceKM.Value,
                DistanceNM = entity.DistanceNM.Value,
                Source = entity.Source,
                FromPortName = entity.FromPort.LocationName,
                ToPortName = entity.ToPort.LocationName,
                TransPortTypeId = entity.TransportTypeId,
                TransPortTypeName = entity.TransportType.TransportTypeName
            };
        }


        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Distance not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService?.UserId;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Distance not found.");

            //entity.IsActive = !entity.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService?.UserId;

            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}
