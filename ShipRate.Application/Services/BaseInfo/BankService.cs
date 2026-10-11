using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Validations.Domain;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _repository;
        public BankService(IBankRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<BankDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.BankName.ToLower().Contains(term) ||
                    x.BankCode.ToLower().Contains(term) ||
                    x.BankLocalName.ToLower().Contains(term));
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

            return new PagedResultDto<BankDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<PagedResultDto<BankDto>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(x =>
                    x.BankName.ToLower().Contains(term) ||
                    x.BankCode.ToLower().Contains(term) ||
                    x.BankLocalName.ToLower().Contains(term));
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

            return new PagedResultDto<BankDto>(items, totalCount, page, pageSize);
        }




        public async Task<BankDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithCountryAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(BankCreateEditDto dto)
        {



            if (await _repository.AnyAsync(x => x.BankCode == dto.BankCode))
                return (false, $"Bank Code '{dto.BankCode}' already exists.");


            var entity = new Bank
            {
                BankCode = dto.BankCode,
                BankName = dto.BankName,
                BankLocalName = dto.BankLocalName,
                SWIFTCode = dto.SWIFTCode,
                Remarks = dto.Remarks,
                CountryId = dto.CountryId,
                IsActive = true,
                IsDeleted = false
            };
            var validation = new BankVaidation().Validate(entity);
            if (!validation.IsValid)
            {
                string errors = "";
                foreach (var err in validation.Errors)
                {
                    errors = errors + err + "-";
                }
                return (false, errors);
            }
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }





        public async Task<(bool Success, string? Error)> UpdateAsync(BankCreateEditDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id.Value);
            if (entity == null || entity.IsDeleted)
                return (false, "Bank not found.");


            entity.CountryId = dto.CountryId;
            entity.BankCode = dto.BankCode;
            entity.BankName = dto.BankName;
            entity.BankLocalName = dto.BankLocalName;
            entity.Remarks = dto.Remarks;
            var validation = new BankVaidation().Validate(entity);
            if (!validation.IsValid)
            {
                string errors = "";
                foreach (var err in validation.Errors)
                {
                    errors = errors + err + "-";
                }
                return (false, errors);
            }
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
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }


        private static BankDto MapToDto(Bank entity)
        {
            return new BankDto
            {
                Id = entity.Id,
                BankName = entity.BankName,
                BankLocalName = entity.BankLocalName,
                CountryName = entity.Country.CountryName,
                CountryId = entity.CountryId,
                BankCode = entity.BankLocalName,
                SWIFTCode = entity.SWIFTCode,
                Remarks = entity.Remarks
            };
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Bank not found.");

            entity.IsDeleted = true;
            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}
