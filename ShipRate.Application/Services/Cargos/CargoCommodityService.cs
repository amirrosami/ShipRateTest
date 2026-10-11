using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Cargo;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Domain.Entities.BaseInfo.Cargos;

namespace ShipRate.Application.Services.Cargos
{
    public class CargoCommodityService : ICargoCommodityService
    {
        private readonly Domain.Interfaces.IRepository<CargoCommodity> _repository;

        public CargoCommodityService(Domain.Interfaces.IRepository<CargoCommodity> repository)
        {
            _repository = repository;
        }
        public async Task<PagedResultDto<CargoCommodityListDto>> GetPagedListAsync(CargoCommoditySearchDto searchDto)
        {

            var query = _repository.GetAllAsQueryable();
            //var query = await _repository.GetAllAsync();

            query = query.Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchDto.SearchName))
            {
                query = query.Where(x => x.CargoCommodityName.Contains(searchDto.SearchName));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchLocalName))
            {
                query = query.Where(x => x.CargoCommodityLocalName.Contains(searchDto.SearchLocalName));
            }

            var totalCount = query.Count();

            query = searchDto.SortColumn?.ToLower() switch
            {
                "cargocommodityname" => searchDto.SortDirection == "asc"
                    ? query.OrderBy(x => x.CargoCommodityName)
                    : query.OrderByDescending(x => x.CargoCommodityName),
                "cargocommoditylocalname" => searchDto.SortDirection == "asc"
                    ? query.OrderBy(x => x.CargoCommodityLocalName)
                    : query.OrderByDescending(x => x.CargoCommodityLocalName),
                _ => query.OrderByDescending(x => x.Id)
            };

            var items = query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(x => new CargoCommodityListDto
                {
                    Id = x.Id,
                    CargoCommodityName = x.CargoCommodityName,
                    CargoCommodityLocalName = x.CargoCommodityLocalName,
                })
                .ToList();

            return new PagedResultDto<CargoCommodityListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize,
                SortColumn = searchDto.SortColumn,
                SortDirection = searchDto.SortDirection
            };
        }

        public async Task<CargoCommodityDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return null;

            return new CargoCommodityDto
            {
                Id = entity.Id,
                CargoCommodityName = entity.CargoCommodityName,
                CargoCommodityLocalName = entity.CargoCommodityLocalName,
            };
        }

        public async Task<CargoCommodityDto> CreateAsync(CargoCommodityDto dto, Guid userId)
        {
            var entity = new CargoCommodity
            {
                Id = Guid.CreateVersion7(),
                CargoCommodityName = dto.CargoCommodityName,
                CargoCommodityLocalName = dto.CargoCommodityLocalName,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            await _repository.AddAsync(entity);
            dto.Id = entity.Id;
            return dto;
        }

        public async Task UpdateAsync(CargoCommodityDto dto, Guid userId)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                throw new Exception("رکورد یافت نشد");

            entity.CargoCommodityName = dto.CargoCommodityName;
            entity.CargoCommodityLocalName = dto.CargoCommodityLocalName;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                throw new Exception("رکورد یافت نشد");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
    }
}
