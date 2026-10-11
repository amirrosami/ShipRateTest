using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class BlackListReasonService : IBlackListReasonService
    {
        private readonly IRepository<BlackListReason> _repository;

        public BlackListReasonService(IRepository<BlackListReason> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<BlackListReasonDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.BlackListReasonName.ToLower().Contains(term));
            }

            // Filter by IsActive
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            // Sorting
            //query = request.SortColumn?.ToLower() switch
            //{
            //    "BlackListReasoncode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.BlackListReasonCode)
            //        : query.OrderBy(x => x.BlackListReasonCode),
            //    "BlackListReasonname" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.BlackListReasonName)
            //        : query.OrderBy(x => x.BlackListReasonName),
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

            return new PagedResultDto<BlackListReasonDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<BlackListReasonDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        //public async Task<BlackListReasonDto> CreateAsync(BlackListReasonCreateDto dto)
        //{
        //    var entity = new BlackListReason
        //    {
        //        Sequence = dto.Sequence,
        //        BlackListReasonCode = dto.BlackListReasonCode.Trim(),
        //        BlackListReasonName = dto.BlackListReasonName.Trim(),
        //        BlackListReasonDescription = dto.BlackListReasonDescription.Trim(),
        //        NumericCode = dto.NumericCode.Trim(),
        //        ENSCode = dto.ENSCode.Trim(),
        //        IsActive = dto.IsActive,
        //        IsDeleted = false
        //    };

        //    await _repository.AddAsync(entity);
        //    await _repository.SaveChangesAsync();

        //    return MapToDto(entity);
        //}

        public async Task<(bool Success, string? Error)> CreateAsync(BlackListReasonCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.BlackListReasonName == dto.BlackListReasonName))
                return (false, $"Name '{dto.BlackListReasonName}' already exists.");


            var entity = new BlackListReason
            {
                BlackListReasonName = dto.BlackListReasonName,
                SystemDefined = false,
                IsActive = dto.IsActive,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }




        public async Task<(bool Success, string? Error)> UpdateAsync(BlackListReasonCreateDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Black List Reason  not found.");



            entity.BlackListReasonName = dto.BlackListReasonName.Trim();
            entity.IsActive = dto.IsActive;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

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

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            return await _repository.AnyAsync(x =>
                x.BlackListReasonName.ToLower() == code.Trim().ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        private static BlackListReasonDto MapToDto(BlackListReason entity)
        {
            return new BlackListReasonDto
            {
                Id = entity.Id,
                BlackListReasonName = entity.BlackListReasonName,
                SystemDefined = entity.SystemDefined,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted
            };
        }


        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Black List Reason  not found.");

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
                return (false, "Balck List reason  not found.");

            entity.IsActive = !entity.IsActive;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}
