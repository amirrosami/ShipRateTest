using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.BaseInfo
{
    public class PackageTypeService : IPackageTypeService
    {
        private readonly IRepository<PackageType> _repository;
        private readonly ApplicationDbContext _db;

        public PackageTypeService(IRepository<PackageType> repository, ApplicationDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<PagedResultDto<PackageTypeDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.PackageTypeName.ToLower().Contains(term) ||
                    x.PackageTypeCode.ToLower().Contains(term) ||
                    x.NumericCode.ToLower().Contains(term) ||
                    x.ENSCode.ToLower().Contains(term) ||
                    x.PackageTypeDescription.ToLower().Contains(term));
            }

            // Filter by IsActive
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            // Sorting
            query = request.SortColumn?.ToLower() switch
            {
                "packagetypecode" => request.SortDirection == "desc"
                    ? query.OrderByDescending(x => x.PackageTypeCode)
                    : query.OrderBy(x => x.PackageTypeCode),
                "packagetypename" => request.SortDirection == "desc"
                    ? query.OrderByDescending(x => x.PackageTypeName)
                    : query.OrderBy(x => x.PackageTypeName),
                "numericcode" => request.SortDirection == "desc"
                    ? query.OrderByDescending(x => x.NumericCode)
                    : query.OrderBy(x => x.NumericCode),
                "enscode" => request.SortDirection == "desc"
                    ? query.OrderByDescending(x => x.ENSCode)
                    : query.OrderBy(x => x.ENSCode),
                "isactive" => request.SortDirection == "desc"
                    ? query.OrderByDescending(x => x.IsActive)
                    : query.OrderBy(x => x.IsActive),
                _ => request.SortDirection == "desc"
                    ? query.OrderByDescending(x => x.Sequence)
                    : query.OrderBy(x => x.Sequence)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<PackageTypeDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<PagedResultDto<PackageTypeDto>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(x =>
                    x.PackageTypeName.ToLower().Contains(term) ||
                    x.PackageTypeCode.ToLower().Contains(term) ||
                    x.NumericCode.ToLower().Contains(term) ||
                    x.ENSCode.ToLower().Contains(term) ||
                    x.PackageTypeDescription.ToLower().Contains(term));
            }

            //// Filter by IsActive
            //if (request.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == request.IsActive.Value);
            //}

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "packagetypecode" => sortDir == "desc"
                    ? query.OrderByDescending(x => x.PackageTypeCode)
                    : query.OrderBy(x => x.PackageTypeCode),
                "packagetypename" => sortDir == "desc"
                    ? query.OrderByDescending(x => x.PackageTypeName)
                    : query.OrderBy(x => x.PackageTypeName),
                "numericcode" => sortDir == "desc"
                    ? query.OrderByDescending(x => x.NumericCode)
                    : query.OrderBy(x => x.NumericCode),
                "enscode" => sortDir == "desc"
                    ? query.OrderByDescending(x => x.ENSCode)
                    : query.OrderBy(x => x.ENSCode),
                "isactive" => sortDir == "desc"
                    ? query.OrderByDescending(x => x.IsActive)
                    : query.OrderBy(x => x.IsActive),
                _ => sortDir == "desc"
                    ? query.OrderByDescending(x => x.Sequence)
                    : query.OrderBy(x => x.Sequence)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<PackageTypeDto>(items, totalCount, page, pageSize);
        }




        public async Task<PackageTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        //public async Task<PackageTypeDto> CreateAsync(PackageTypeCreateDto dto)
        //{
        //    var entity = new PackageType
        //    {
        //        Sequence = dto.Sequence,
        //        PackageTypeCode = dto.PackageTypeCode.Trim(),
        //        PackageTypeName = dto.PackageTypeName.Trim(),
        //        PackageTypeDescription = dto.PackageTypeDescription.Trim(),
        //        NumericCode = dto.NumericCode.Trim(),
        //        ENSCode = dto.ENSCode.Trim(),
        //        IsActive = dto.IsActive,
        //        IsDeleted = false
        //    };

        //    await _repository.AddAsync(entity);
        //    await _repository.SaveChangesAsync();

        //    return MapToDto(entity);
        //}

        public async Task<(bool Success, string? Error)> CreateAsync(PackageTypeCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.PackageTypeCode == dto.PackageTypeCode))
                return (false, $"Code '{dto.PackageTypeCode}' already exists.");


            var entity = new PackageType
            {
                Sequence = dto.Sequence,
                PackageTypeCode = dto.PackageTypeCode.Trim(),
                PackageTypeName = dto.PackageTypeName.Trim(),
                PackageTypeDescription = dto.PackageTypeDescription.Trim(),
                NumericCode = dto.NumericCode.Trim(),
                ENSCode = dto.ENSCode.Trim(),
                IsActive = dto.IsActive,
                IsDeleted = false
            };

            var validation = new PackageTypesValidation().Validate(entity);
            if (!validation.IsValid)
            {
                string errors = "";
                foreach (var err in validation.Errors)
                {
                    errors = errors + "-" + err;
                }
                return (false, errors);
            }
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }




        //public async Task<PackageTypeDto?> UpdateAsync(PackageTypeUpdateDto dto)
        //{
        //    var entity = await _repository.GetByIdAsync(dto.Id);
        //    if (entity == null) return null;

        //    entity.Sequence = dto.Sequence;
        //    entity.PackageTypeCode = dto.PackageTypeCode.Trim();
        //    entity.PackageTypeName = dto.PackageTypeName.Trim();
        //    entity.PackageTypeDescription = dto.PackageTypeDescription.Trim();
        //    entity.NumericCode = dto.NumericCode.Trim();
        //    entity.ENSCode = dto.ENSCode.Trim();
        //    entity.IsActive = dto.IsActive;

        //    await _repository.UpdateAsync(entity);
        //    await _repository.SaveChangesAsync();

        //    return MapToDto(entity);
        //}

        public async Task<(bool Success, string? Error)> UpdateAsync(PackageTypeCreateDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Package type not found.");



            entity.Sequence = dto.Sequence;
            entity.PackageTypeCode = dto.PackageTypeCode.Trim();
            entity.PackageTypeName = dto.PackageTypeName.Trim();
            entity.PackageTypeDescription = dto.PackageTypeDescription.Trim();
            entity.NumericCode = dto.NumericCode.Trim();
            entity.ENSCode = dto.ENSCode.Trim();
            entity.IsActive = dto.IsActive;
            var validation = new PackageTypesValidation().Validate(entity);
            if (!validation.IsValid)
            {
                string errors = "";
                foreach (var err in validation.Errors)
                {
                    errors = errors + "-" + err;
                }
                return (false, errors);
            }
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

        //public async Task<bool> ToggleActiveAsync(int id)
        //{
        //    var entity = await _repository.GetByIdAsync(id);
        //    if (entity == null) return false;

        //    entity.IsActive = !entity.IsActive;
        //    await _repository.UpdateAsync(entity);
        //    await _repository.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            return await _repository.AnyAsync(x =>
                x.PackageTypeCode.ToLower() == code.Trim().ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        private static PackageTypeDto MapToDto(PackageType entity)
        {
            return new PackageTypeDto
            {
                Id = entity.Id,
                Sequence = entity.Sequence,
                PackageTypeCode = entity.PackageTypeCode,
                PackageTypeName = entity.PackageTypeName,
                PackageTypeDescription = entity.PackageTypeDescription,
                NumericCode = entity.NumericCode,
                ENSCode = entity.ENSCode,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted
            };
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var trimmed = code.Trim().ToLower();
            if (excludeId.HasValue)
            {
                return !await _repository.AnyAsync(x => x.PackageTypeCode.ToLower() == trimmed && x.Id != excludeId.Value);
            }
            return !await _repository.AnyAsync(x => x.PackageTypeCode.ToLower() == trimmed);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Package type not found.");

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
                return (false, "Package type not found.");

            entity.IsActive = !entity.IsActive;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<IEnumerable<NLookupDto>> SearchPackageTypesAsync(string? term, int take = 20)
        {
            if (take <= 0) take = 20;
            if (take > 100) take = 100;

            var query = _db.PackageTypes
                .AsNoTracking()
                .AsQueryable();

            // فیلتر کردن بر اساس عبارت جستجو شده
            if (!string.IsNullOrWhiteSpace(term))
            {
                var trimmedTerm = term.Trim();

                // جستجو همزمان روی PortCode (مانند CNSHA یا IRBND) و PortName
                query = query.Where(p =>
                    p.PackageTypeName.Contains(trimmedTerm) ||
                    (p.PackageTypeCode != null && p.PackageTypeCode.Contains(trimmedTerm)));
            }

            return await query
                .OrderBy(p => p.PackageTypeName)
                .Take(take)
                .Select(p => new NLookupDto
                {
                    Value = p.Id.ToString(), // تبدیل Id (چه Guid چه int) به string
                    Text = p.PackageTypeName
                })
                .ToListAsync();
        }









    }
}
