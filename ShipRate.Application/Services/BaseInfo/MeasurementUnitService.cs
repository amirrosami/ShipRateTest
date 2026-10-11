using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class MeasurementUnitService : IMeasurementUnitService
    {
        private readonly IMeasurementUnitRepository _repository;
        private readonly IRepository<MeasurementUnitCategory> _categoryrepository;

        public MeasurementUnitService(IMeasurementUnitRepository repository, IRepository<MeasurementUnitCategory> categoryrepository)
        {
            _repository = repository;
            _categoryrepository = categoryrepository;
        }

        public async Task<PagedResultDto<MeasurementUnitDTO>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.MeasurementUnitName.ToLower().Contains(term) ||
                    x.MeasurementUnitLocalName.ToLower().Contains(term));
            }

            // Filter by IsActive
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

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

            return new PagedResultDto<MeasurementUnitDTO>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<PagedResultDto<MeasurementUnitDTO>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(x =>
                    x.MeasurementUnitName.ToLower().Contains(term) ||
                    x.MeasurementUnitLocalName.ToLower().Contains(term));
            }

            //// Filter by IsActive
            //if (request.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == request.IsActive.Value);
            //}

            // Sorting
            //query = sortBy?.ToLower() switch
            //{
            //    "packagetypecode" => sortDir == "desc"
            //        ? query.OrderByDescending(x => x.PackageTypeCode)
            //        : query.OrderBy(x => x.PackageTypeCode),
            //    "packagetypename" => sortDir == "desc"
            //        ? query.OrderByDescending(x => x.PackageTypeName)
            //        : query.OrderBy(x => x.PackageTypeName),
            //    "numericcode" => sortDir == "desc"
            //        ? query.OrderByDescending(x => x.NumericCode)
            //        : query.OrderBy(x => x.NumericCode),
            //    "enscode" => sortDir == "desc"
            //        ? query.OrderByDescending(x => x.ENSCode)
            //        : query.OrderBy(x => x.ENSCode),
            //    "isactive" => sortDir == "desc"
            //        ? query.OrderByDescending(x => x.IsActive)
            //        : query.OrderBy(x => x.IsActive),
            //    _ => sortDir == "desc"
            //        ? query.OrderByDescending(x => x.Sequence)
            //        : query.OrderBy(x => x.Sequence)
            //};

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<MeasurementUnitDTO>(items, totalCount, page, pageSize);
        }




        public async Task<MeasurementUnitDTO?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdWithCategoryAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(MeasurementUnitCreateDTO dto)
        {

            if (await _repository.AnyAsync(x => x.MeasurementUnitName == dto.UnitName))
                return (false, $"Unit Name '{dto.UnitName}' already exists.");


            var entity = new MeasurementUnit
            {
                MeasurementUnitName = dto.UnitName,
                MeasurementUnitLocalName = dto.UnitLocalName,
                MeasurementUnitCategoryId = dto.CategoryId,
                SystemDefined = false,
                IsActive = true,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }





        public async Task<(bool Success, string? Error)> UpdateAsync(MeasurementUnitCreateDTO dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Measurement Unit not found.");


            entity.MeasurementUnitCategoryId = dto.CategoryId;
            entity.MeasurementUnitName = dto.UnitName;
            entity.MeasurementUnitLocalName = dto.UnitLocalName;
            entity.IsActive = dto.IsActive;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);
        }





        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        //public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        //{
        //    return await _repository.AnyAsync(x =>
        //        x.PackageTypeCode.ToLower() == code.Trim().ToLower() &&
        //        (!excludeId.HasValue || x.Id != excludeId.Value));
        //}

        private static MeasurementUnitDTO MapToDto(MeasurementUnit entity)
        {
            return new MeasurementUnitDTO
            {
                Id = entity.Id,
                MeasurementUnitCategoryId = entity.MeasurementUnitCategoryId,
                MeasurementUnitName = entity.MeasurementUnitName,
                MeasurementUnitLocalName = entity.MeasurementUnitLocalName,
                MeasurementUnitCategoryName = entity.MeasurementUnitCategory.MeasurementUnitCategoryName,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                SystemDefined = entity.SystemDefined
            };
        }

        //public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        //{
        //    var trimmed = code.Trim().ToLower();
        //    if (excludeId.HasValue)
        //    {
        //        return !await _repository.AnyAsync(x => x.PackageTypeCode.ToLower() == trimmed && x.Id != excludeId.Value);
        //    }
        //    return !await _repository.AnyAsync(x => x.PackageTypeCode.ToLower() == trimmed);
        //}

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Measurement unit not found.");

            entity.IsDeleted = true;
            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Measurement Unit not found.");

            entity.IsActive = !entity.IsActive;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<IEnumerable<MeasurementUnitCategory>> GetCategoriesAsync()
        {
            return await _categoryrepository.GetAllAsync();
        }
    }
}
